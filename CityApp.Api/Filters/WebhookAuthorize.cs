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
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Filters
{
    /// <summary>
    /// Authorization Filter for WebhookController
    /// </summary>
    public class WebhookAuthorize : TypeFilterAttribute
    {
        public WebhookAuthorize()
            : base(typeof(WebhookAuthorizeFilterImpl))
        { }

        private class WebhookAuthorizeFilterImpl :  IAsyncActionFilter
        {
            private static readonly ILogger _logger = Log.Logger.ForContext<WebhookAuthorizeFilterImpl>();

            private readonly AppSettings _appSettings;

            public WebhookAuthorizeFilterImpl(IOptions<AppSettings> appSettings)
            {
                _appSettings = appSettings.Value;
            }


            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                string authHeader = context.HttpContext.Request.Headers["Authorization"];

                //Check if we have an AuthorizationToken
                if (string.IsNullOrWhiteSpace(authHeader))
                {
                    context.Result = ErrorResult(ErrorCode.Forbidden, "Invalid credentials.");
                    return;
                }

                string token = authHeader.Split(' ').Last();
                //Check if the token matches the appsettings token
                if (token != _appSettings.WebhookToken)
                {
                    context.Result = ErrorResult(ErrorCode.Forbidden, "Invalid credentials.");
                    return;
                }

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
