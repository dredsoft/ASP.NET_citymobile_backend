using AutoMapper;
using CityApp.Common.Caching;
using CityApp.Common.Models;
using CityApp.Data;
using CityApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Services
{
    public class CommonService : ICustomService
    {
        private static readonly TimeSpan _bradyExpiry = TimeSpan.FromDays(30);

        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        private static readonly object _randomLock = new object();

        private readonly CommonContext _commonCtx;
        private readonly RedisCache _cache;
        private readonly IMapper _mapper;

        private static readonly ILogger _logger = Log.Logger.ForContext<CommonService>();


        public CommonService(CommonContext commonCtx, RedisCache cache, IMapper mapper)
        {
            _commonCtx = commonCtx;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<LoggedInUser> GetLoggedInUserAsync(Guid loggedInUserId)
        {
            var cacheKey = WebCacheKey.LoggedInUser(loggedInUserId);
            var expiry = TimeSpan.FromHours(1);

            var loggedInUser = await _cache.GetWithSlidingExpirationAsync<LoggedInUser>(cacheKey, expiry);
            if (loggedInUser != null)
            {
                return loggedInUser;
            }

            try
            {
                var commonUser = await _commonCtx.Users.SingleOrDefaultAsync(u => u.Id == loggedInUserId);
                if (commonUser == null)
                {
                    _logger.Error($"No matching {nameof(CommonUser)} record for loggedInUserId={loggedInUserId}");
                    return null;
                }

                loggedInUser = Mapper.Map<LoggedInUser>(commonUser);

                await _cache.SetAsync(cacheKey, loggedInUser, expiry);
            }
            catch( Exception ex)
            {

            }

            return loggedInUser;
        }


    }
}
