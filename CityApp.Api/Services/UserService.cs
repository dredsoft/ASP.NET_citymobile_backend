using AutoMapper;
using CityApp.Api.Models;
using CityApp.Common.Caching;
using CityApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Api.Services
{
    public class UserService : ICustomWebService
    {
        private readonly CommonContext _commonContext;
        private readonly RedisCache _cache;

        public UserService(CommonContext commonCtx, RedisCache cache)
        {
            _commonContext = commonCtx;
            _cache = cache;
        }

        /// <summary>
        /// Looks for the accountuser in cache.  If it doesn't exists, looks for accountuser in database.  If found, adds it to cache.
        /// </summary>
        public async Task<CommonUserAccountModel> GetCommonAccountUserAsync(long accountNumber, Guid userId)
        {
            var cacheKey = WebCacheKey.CommonUserAccount(accountNumber, userId);
            var expiry = TimeSpan.FromHours(24);

            // Try to pull it from the cache first.
            var commonUserAccountModel = await _cache.GetWithSlidingExpirationAsync<CommonUserAccountModel>(cacheKey, expiry);
            if (commonUserAccountModel != null)
            {
                return commonUserAccountModel;
            }

            var commonUserAccount = await _commonContext.UserAccounts
               .AsNoTracking()
               .SingleOrDefaultAsync(cua => cua.UserId == userId && cua.Account.Number == accountNumber);

            if (commonUserAccount != null)
            {
                commonUserAccountModel = Mapper.Map<CommonUserAccountModel>(commonUserAccount);

                // Found it. Add it to the cache.
                await _cache.SetAsync(cacheKey, commonUserAccountModel, expiry);
            }

            return commonUserAccountModel;
        }
    }

}
