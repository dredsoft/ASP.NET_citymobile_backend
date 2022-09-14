using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Filters
{
    public abstract class BaseTypeFilterImpl
    {
        /// <summary>
        /// Ensure the user is signed out, then redirect them to the login page.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected async Task<IActionResult> SignOutAsync(FilterContext context)
        {
            await context.HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return new RedirectToActionResult("Login", "User", null);
        }

        /// <summary>
        /// Send the user to /Accounts, where they can view all of their accounts.
        /// </summary>
        /// <returns></returns>
        protected IActionResult RedirectUserToAccountList()
        {
            return new RedirectToActionResult("Index", "Accounts", null);
        }
    }
}
