using CityApp.Common.Extensions;
using CityApp.Data.Enums;
using CityApp.Data.Extensions;
using CityApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Filters
{
    /// <summary>
    /// Authorization attribute filter.
    /// Credit: http://benjamincollins.com/blog/practical-permission-based-authorization-in-asp-net-core/
    /// </summary>
    public class RequiresPermissionAttribute : TypeFilterAttribute
    {
        public RequiresPermissionAttribute(params AccountPermissions[] permissions)
          : base(typeof(RequiresPermissionAttributeImpl))
        {
            Arguments = new[] { new PermissionsAuthorizationRequirement(permissions) };
        }

        private class RequiresPermissionAttributeImpl : BaseTypeFilterImpl, IAsyncResourceFilter
        {
            private readonly PermissionsAuthorizationRequirement _requiredPermissions;
            private readonly UserService _userSvc;
            private readonly string _notAuthorizedMessage = "Not authorized to perform this action.";

            public RequiresPermissionAttributeImpl(PermissionsAuthorizationRequirement requiredPermissions, UserService userSvc)
            {
                _requiredPermissions = requiredPermissions;
                _userSvc = userSvc;
            }

            public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
            {
                // If not logged in, send the user to the login page. This attribute is only applied to AccountBaseController,
                //   so the must must be logged in to access these pages.
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = await SignOutAsync(context);
                    return;
                }

                var loggedInUserId = context.HttpContext.User.GetLoggedInUserId();
                if (loggedInUserId == null)
                {
                    context.Result = await SignOutAsync(context);
                    return;
                }

                // This filter is applied to AccountBaseController, which has URLs containing the account number. If 
                //   we are unable to grab the account number from route data, send the user to the account list page.
                var accountNumberFromRoute = context.RouteData.GetAccountNumberFromRoute();
                if (accountNumberFromRoute == null)
                {
                    context.Result = RedirectUserToAccountList();
                    return;
                }

                var accountUser = await _userSvc.GetCommonAccountUserAsync(accountNumberFromRoute.Value, loggedInUserId.Value);
                if (!accountUser.Permissions.HasAllPermissions(_requiredPermissions.RequiredPermissions.ToArray()))
                {
                    context.Result = context.HttpContext.Request.IsAjaxRequest()
                        ? context.ModelState.ToJsonErrorResult(new[] { _notAuthorizedMessage })
                        : RedirectToNotAuthorized(accountNumberFromRoute.Value);

                    return;
                }

                // Let the request continue.
                await next();
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
        }
    }


    public class PermissionsAuthorizationRequirement : IAuthorizationRequirement
    {
        public IEnumerable<AccountPermissions> RequiredPermissions { get; }

        public PermissionsAuthorizationRequirement(IEnumerable<AccountPermissions> requiredPermissions)
        {
            RequiredPermissions = requiredPermissions;
        }
    }
}
