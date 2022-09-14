using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using CityApp.Data;
using CityApp.Common.Caching;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading.Tasks;
using CityApp.Services;
using CityApp.Common.Extensions;
using CityApp.Common.Enums;
using CityApp.Api.Models;
using System.Collections.Generic;

namespace CityApp.Api.Controllers
{
    public class BaseApiController : Controller
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<BaseApiController>();

        protected CommonContext CommonContext { get; private set; }
        protected AppSettings AppSettings { get; private set; }

        protected readonly RedisCache _cache;


        /// <summary>
        /// <para>If not null, the logged in user's basic details mapped from <see cref="CommonUser" />.</para>
        /// <para>This property WILL be null for anonymous pages (Plans, External 4473, etc.)</para>
        /// </summary>
        protected LoggedInUser LoggedInUser
        {
            get
            {
                if (_loggedInUser == null)
                {
                    //TODO: Adding a result here could cause problems.  We'll have to watch this.
                    //We may need to create a GetLoggedInUser without the async
                    _loggedInUser = GetLoggedInUserAsync(User).Result;
                }

                return _loggedInUser;
            }
        }

        private LoggedInUser _loggedInUser { get; set; }

        public string UserId
        {
            get
            {
                if (User != null && User.GetJWTLoggedInUserId() != null)
                {
                    return User.GetJWTLoggedInUserId().ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        public BaseApiController(CommonContext commonContext, RedisCache cache, IOptions<AppSettings> appSettings)
        {
            CommonContext = commonContext;
            _cache = cache;
            AppSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [Route("Status")]
        public string Status() => "Good";

        [Authorize]
        [Route("Claims")]
        public object Claims()
        {
            return User.Claims.Select(c =>
            new
            {
                Type = c.Type,
                Value = c.Value
            });
        }

        /// <summary>
        /// ASP.NET Core says we're logged in. Try to load the user's id from the cookie, and the user's details from 
        /// either cache or the database.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected async Task<LoggedInUser> GetLoggedInUserAsync(ClaimsPrincipal user)
        {
            var loggedInUserId = user.GetJWTLoggedInUserId();
            if (loggedInUserId == null)
            {
                return null;
            }

            var commonService = this.HttpContext.RequestServices.GetRequiredService<CommonService>();
            var loggedInUser = await commonService.GetLoggedInUserAsync(loggedInUserId.Value);

            return loggedInUser;
        }

        public IActionResult ErrorResult(ErrorCode code, string message)
        {
            var errors = new Error { Code = code, Message = message };
            var response = new APIResponse<string>() { Success = false, Errors = new List<Error> { errors } };

            var jsonResult = new JsonResult(response);

            return jsonResult;
        }
    }

}
