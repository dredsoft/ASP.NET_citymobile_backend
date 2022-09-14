using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using CityApp.Common.Utilities;
using CityApp.Common.Serialization;

namespace CityApp.Common.Caching
{
    /// <summary>
    /// <para>!!! IMPORTANT !!! This class is only intended to be used as a singleton because we construct the
    /// multiplexer in our constructor, and constructing multiplexers is expensive.</para>
    /// <para>We have to use an instance created via DI instead of a static class because we need to be able 
    /// inject Configuration to have access to the connection string.</para>
    /// </summary>
    public class RedisCache
    {
        private static readonly ILogger _logger = Log.ForContext<RedisCache>();
        private string redisConnectionString;
        protected ConnectionMultiplexer Multiplexer
        { get; set; }

        public RedisCache(IConfiguration config)
        {
            redisConnectionString = config.GetConnectionString("Redis");
            // Be sure to set abortConnect=false in the connection string so that we gracefully handle connection failures.
            Multiplexer = InitializeConnectionMultiplexer(ConfigurationOptions.Parse(redisConnectionString));
        }

        protected ConnectionMultiplexer InitializeConnectionMultiplexer(ConfigurationOptions options)
        {
            // Be sure to set abortConnect=false in the connection string so that we gracefully handle connection failures.
            var multiplexer = ConnectionMultiplexer.Connect(options);

            // So we can tell which one failed - regular or admin.
            var logPrefix = options.AllowAdmin ? "Admin" : "";

            // Log connection and error events.
            multiplexer.ConnectionFailed += (sender, e) =>
            {
                _logger.Verbose(e.Exception, $"{logPrefix}ConnectionFailed: Connection type '{e.ConnectionType}' on EndPoint '{e.EndPoint}' reported ConnectionFailureType '{e.FailureType}'");
            };

            multiplexer.ConnectionRestored += (sender, e) =>
            {
                _logger.Verbose(e.Exception, $"{logPrefix}ConnectionRestored: Connection type '{e.ConnectionType}' on EndPoint '{e.EndPoint}' reported ConnectionFailureType '{e.FailureType}'");
            };

            multiplexer.ErrorMessage += (sender, e) =>
            {
                _logger.Verbose($"{logPrefix}ErrorMessage: Server '{e.EndPoint.ToString()}' reported this error message: {e.Message}");
            };

            return multiplexer;
        }
        
        public async Task FlushDB()
        {
            var server = Multiplexer.GetServer(redisConnectionString);
            await server.FlushAllDatabasesAsync();
        }

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key)
        {
            Check.NotEmpty(key, nameof(key));

            try
            {
                var db = GetDatabase();

                var value = await db
                    .StringGetAsync(key)
                    .ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return JsonNet.Deserialize<T>(value);
                }
            }
            catch (Exception ex)
            {
                // Don't let cache server unavailability bring down the application.
                _logger.Error(ex, $"Unhandled exception trying to async GET '{key}'");
            }

            return default(T);
        }


        #region Sliding Expiration Lua script

        // If the value exists, update the key's expiration.
        //   * KEYS[1]: the key
        //   * ARGV[1]: the new expiration time in seconds
        private const string SLIDING_EXPIRATION_LUA_SCRIPT = @"
local val = redis.call('GET', KEYS[1])
if val ~= false then
	redis.call('EXPIRE', KEYS[1], ARGV[1])
end

return val
";

        #endregion

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned. If the key does exist,
        /// set or update its expiration time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<T> GetWithSlidingExpirationAsync<T>(string key, TimeSpan expiry)
        {
            Check.NotEmpty(key, nameof(key));

            try
            {
                var db = GetDatabase();

                var redisResult = await db
                    .ScriptEvaluateAsync(SLIDING_EXPIRATION_LUA_SCRIPT, new RedisKey[] { key }, new RedisValue[] { expiry.TotalSeconds })
                    .ConfigureAwait(false);

                var value = (string)redisResult;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return JsonNet.Deserialize<T>(value);
                }
            }
            catch (Exception ex)
            {
                // Don't let cache server unavailability bring down the application.
                _logger.Error(ex, $"Unhandled exception trying to async GET/EXPIRE '{key}'");
            }

            return default(T);
        }

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned. If the key does exist,
        /// set or update its expiration time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public T GetWithSlidingExpiration<T>(string key, TimeSpan expiry)
        {
            Check.NotEmpty(key, nameof(key));

            try
            {
                var db = GetDatabase();

                var redisResult = db
                    .ScriptEvaluate(SLIDING_EXPIRATION_LUA_SCRIPT, new RedisKey[] { key }, new RedisValue[] { expiry.TotalSeconds });

                var value = (string)redisResult;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return JsonNet.Deserialize<T>(value);
                }
            }
            catch (Exception ex)
            {
                // Don't let cache server unavailability bring down the application.
                _logger.Error(ex, $"Unhandled exception trying to GET/EXPIRE '{key}'");
            }

            return default(T);
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Any previous 
        /// time to live associated with the key is discarded on successful SET operation.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> SetAsync(string key, object value, TimeSpan? expiry = null)
        {
            Check.NotEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));

            try
            {
                var db = GetDatabase();

                return await db
                    .StringSetAsync(key, JsonNet.Serialize(value), expiry)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Don't let cache server unavailability bring down the application.
                _logger.Error(ex, $"Unhandled exception trying to async SET '{key}'");
            }

            return false;
        }

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten, regardless of its type. Any previous 
        /// time to live associated with the key is discarded on successful SET operation.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Set(string key, object value, TimeSpan? expiry = null)
        {
            Check.NotEmpty(key, nameof(key));
            Check.NotNull(value, nameof(value));

            try
            {
                var db = GetDatabase();

                return db.StringSet(key, JsonNet.Serialize(value), expiry);
            }
            catch (Exception ex)
            {
                // Don't let cache server unavailability bring down the application.
                _logger.Error(ex, $"Unhandled exception trying to SET '{key}'");
            }

            return false;
        }

        /// <summary>
        /// Removes the specified key. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(string key)
        {
            Check.NotEmpty(key, nameof(key));

            try
            {
                var db = GetDatabase();

                return await db
                    .KeyDeleteAsync(key)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Don't let cache server unavailability bring down the application.
                _logger.Error(ex, $"Unhandled exception trying to async DEL {key}");
            }

            return false;
        }

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task<long> RemoveAsync(string[] keys)
        {
            Check.NotEmpty(keys, nameof(keys));
            Check.HasNoEmpties(keys, nameof(keys));

            try
            {
                var db = GetDatabase();

                var redisKeys = keys
                    .Select(k => (RedisKey)k)
                    .ToArray();

                return await db
                    .KeyDeleteAsync(redisKeys)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Don't let cache server unavailability bring down the application.
                _logger.Error(ex, $"Unhandled exception trying to async DEL {string.Join(" ", keys)}");
            }

            return 0L;
        }


        private IDatabase GetDatabase()
        {
            return Multiplexer.GetDatabase();
        }
    }
}
