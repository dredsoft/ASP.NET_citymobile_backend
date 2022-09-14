using System;
using Microsoft.AspNetCore.Mvc;
using CityApp.Data;
using Serilog;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using CityApp.Common.Caching;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using CityApp.Web.Constants;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using CityApp.Services;
using CityApp.Common.Extensions;
using CityApp.Data.Models;
using CityApp.Web.Models;
using System.Linq;
using CityApp.Data.Enums;
using CityApp.Common.Utilities;

namespace CityApp.Web.Controllers
{
    public class BaseController : Controller
    {
        private static readonly ILogger _logger = Log.ForContext<BaseController>();

        protected readonly RedisCache _cache;
        private readonly IServiceProvider _serviceProvider;

        protected CommonContext CommonContext { get; private set; }
       

        protected AppSettings AppSettings { get; private set; }

        public BaseController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache cache, IOptions<AppSettings> appSettings)
        {
            CommonContext = commonContext;
            _cache = cache;
            _serviceProvider = serviceProvider;
            AppSettings = appSettings.Value;
            
        }

        /// <summary>
        /// <para>If not null, the logged in user's basic details mapped from <see cref="CommonUser" />.</para>
        /// <para>This property WILL be null for anonymous pages (Plans, External 4473, etc.)</para>
        /// </summary>
        protected LoggedInUser LoggedInUser { get; private set; }

        #region Method Overrides

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (User.Identity.IsAuthenticated)
            {
                // Load the logged in user's details and save on the controller for actions to use.
                LoggedInUser = await GetLoggedInUserAsync(User);
                if (LoggedInUser == null)
                {
                    _logger.Warning($"ASP.NET Core says the user is authenticated, but we couldn't load a LoggedInUser object for UserId={User.GetLoggedInUserId()}.");
                    context.Result = await SignOutAsync(context);
                    return;
                }

                ViewData[ViewDataKey.LoggedInUser] = LoggedInUser;
            }

            // Let the base class do its thing.
            await base.OnActionExecutionAsync(context, next);
        }

        #endregion

        //
        // Protected Methods
        //

        private async Task<IActionResult> SignOutAsync(ActionExecutingContext context)
        {
            await context.HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var controller = (Controller)context.Controller;
            return controller.RedirectToAction(nameof(HomeController.Index), "Home");
        }

        protected void ApplyVideoAndImageUrl(Citation citation, CitationViolationListItem model, FileService fileService)
        {

            if (citation.Attachments.Count > 0)
            {
                var citationAttachment = citation.Attachments.OrderByDescending(s => s.CreateUtc).ToList();

                foreach (var ca in citationAttachment)
                {
                    if (ca.Attachment.AttachmentType == CitationAttachmentType.Video)
                    {
                        //Read the file from AWS Bucket
                        model.VideoUrl = fileService.ReadFileUrl(ca.Attachment.Key, AppSettings.AWSAccessKeyID, AppSettings.AWSSecretKey, AppSettings.AmazonS3Bucket);
                        //model.VideoUrl = AWSHelper.GetS3Url(ca.Attachment.Key, _appSettings.AmazonS3Url);
                        model.VideoAttachmentId = ca.Attachment.Id;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(model.VideoUrl))
                {
                    foreach (var ca in citationAttachment)
                    {
                        if (ca.Attachment.AttachmentType == CitationAttachmentType.Image)
                        {
                            //Read the file from AWS Bucket
                            // model.ImageUrl = _fileService.ReadFileUrl(ca.Attachment.Key, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);
                            model.ImageUrl = AWSHelper.GetS3Url(ca.Attachment.Key, AppSettings.AmazonS3Url);
                            break;
                        }
                    }
                }
            }

        }


        /// <summary>
        /// ASP.NET Core says we're logged in. Try to load the user's id from the cookie, and the user's details from 
        /// either cache or the database.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected async Task<LoggedInUser> GetLoggedInUserAsync(ClaimsPrincipal user)
        {
            var loggedInUserId = user.GetLoggedInUserId();
            if (loggedInUserId == null)
            {
                return null;
            }

            var commonService = this.HttpContext.RequestServices.GetRequiredService<CommonService>();
            var loggedInUser = await commonService.GetLoggedInUserAsync(loggedInUserId.Value);

            return loggedInUser;
        }

        protected async Task PurgeLoggedInUser()
        {
            await PurgeLoggedInUser(LoggedInUser.Id);
        }

        protected async Task PurgeLoggedInUser(Guid id)
        {
            var cacheKey = WebCacheKey.LoggedInUser(id);
            await _cache.RemoveAsync(cacheKey);
        }
    }
}
