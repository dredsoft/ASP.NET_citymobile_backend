using CityApp.Api.Controllers;
using CityApp.Api.Models;
using CityApp.Api.Services;
using CityApp.Common.Caching;
using CityApp.Common.Enums;
using CityApp.Common.Extensions;
using CityApp.Common.Models;
using CityApp.Data;
using CityApp.Data.Models;
using CityApp.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Filters
{
    /// <summary>
    /// Grab the account number from the URL and load the CommonAccount data. Use the logged in user's Id to verify
    /// the user's ability to access the account data. If successful, store the account data on the controller for
    /// actions to use later in the request.
    /// </summary>
    public class AccountActionFilter : TypeFilterAttribute
    {
        public AccountActionFilter()
            : base(typeof(AccountActionFilterImpl))
        { }

        private class AccountActionFilterImpl :  IAsyncActionFilter
        {
            private static readonly ILogger _logger = Log.Logger.ForContext<AccountActionFilterImpl>();

            private readonly RedisCache _cache;
            private readonly UserService _userSvc;
            private readonly CommonAccountService _commonAccountSvc;
            private readonly IHostingEnvironment _env;

            public AccountActionFilterImpl(CommonContext commonContext, AccountContext accountContext, RedisCache cache, UserService userSvc, CommonAccountService commonAccountSvc, IHostingEnvironment env)
            {
                _cache = cache;
                _userSvc = userSvc;
                _commonAccountSvc = commonAccountSvc;
                _env = env;
            }


            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                // If not logged in, send the user to the login page. This attribute is only applied to AccountBaseController,
                //   so they must must be logged in to access these pages.
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = ErrorResult(ErrorCode.Forbidden, "Invalid credentials.");
                    return;
                }

                var loggedInUserId = context.HttpContext.User.GetJWTLoggedInUserId();
                if (loggedInUserId == null)
                {
                    _logger.Error($"ASP.NET Core says the user is logged in, but we couldn't obtain the logged in user's Id from the ClaimsPrincipal.");

                    context.Result = ErrorResult(ErrorCode.Forbidden, "Invalid credentials.");

                    return;
                }

                // This filter is applied to AccountBaseController, which has URLs containing the account number. If 
                //   we are unable to grab the account number from route data, send the user to the account list page.
                var accountNumberFromRoute = context.RouteData.GetAccountNumberFromRoute();
                if (accountNumberFromRoute == null)
                {

                    context.Result = ErrorResult(ErrorCode.Forbidden, "Invalid Account.");

                    return;
                }

                // We have the account number. Make sure the user has an association with this account.
                var accountUser = await _userSvc.GetCommonAccountUserAsync(accountNumberFromRoute.Value, loggedInUserId.Value);
                if (accountUser == null)
                {
                    _logger.Error($"User.Id={loggedInUserId} does not have a CommonUserAccount record for CommonAccount.Number={accountNumberFromRoute}.");
                    context.Result = ErrorResult(ErrorCode.Forbidden, "You do not have access to this resource.");

                    return;
                }

                

                var cachedAccount = await _commonAccountSvc.GetCachedAccountAsync(accountNumberFromRoute.Value);

                // We have the account number. Now make sure it's valid and the user has access by loading the user's
                //   CommonAccount record. If we can't find such a record, redirect the user to the account list page.
                if (cachedAccount == null)
                {
                    _logger.Error($"User.Id={loggedInUserId.Value} tried to access {nameof(CommonAccount)}.Number={accountNumberFromRoute}, but we could not find that {nameof(CommonAccount)}.");
                    context.Result = ErrorResult(ErrorCode.Forbidden, "Invalid Account.");

                    return;
                }

                // Store this data on the controller for later use by the request.
                var controller = (BaseAccountApiController)context.Controller;
                controller.CommonAccount = cachedAccount;
                controller.CommonUserAccount = accountUser;

                // Let the request continue.
                await next();
            }


           
            private IActionResult ErrorResult(ErrorCode code, string message)
            {
                var errors = new Error {Code =code, Message = message};
                var response = new APIResponse<string>() { Success = false, Errors = new List<Error> {errors } };

                var jsonResult = new JsonResult(response);

                return jsonResult;
            }

        }
    }
}
