using AutoMapper;
using CityApp.Common.Caching;
using CityApp.Common.Models;
using CityApp.Data;
using CityApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Filters
{
    public class BaseAccountActionFilterImpl : BaseTypeFilterImpl
    {
        private readonly AccountContext _accountCtx;
        private readonly CommonContext _commonCtx;
        private readonly RedisCache _cache;

        public BaseAccountActionFilterImpl(AccountContext accountCtx, CommonContext commonCtx, RedisCache cache)
        {
            _accountCtx = accountCtx;
            _commonCtx = commonCtx;
            _cache = cache;
        }

        protected GlobalViewDataModel CreateGlobalViewDataAsync(CachedAccount cachedAccount, CommonUserAccountModel commonUserAccountModel = null)
        {
            var globalViewData = Mapper.Map<GlobalViewDataModel>(cachedAccount);

            if (commonUserAccountModel != null)
            {
                globalViewData.Permissions = commonUserAccountModel.Permissions;
                globalViewData.UserOwnsAnAccount = cachedAccount.OwnerUserId == commonUserAccountModel.UserId;
            }

            return globalViewData;
        }



        protected IActionResult RedirectToNotAuthorized(long accountNumber)
        {
            return new RedirectToRouteResult(
                 new RouteValueDictionary
                 {
                     ["controller"] = "Accounts",
                     ["action"] = "NotAuthorized",
                     ["id"] = accountNumber
                 });
        }

        //protected IActionResult RedirectToSuspended(long accountNumber)
        //{
        //    return new RedirectToRouteResult(
        //         new RouteValueDictionary
        //         {
        //             ["controller"] = "Accounts",
        //             ["action"] = "Suspended",
        //             ["accountNumber"] = accountNumber
        //         });
        //}

    }
}
