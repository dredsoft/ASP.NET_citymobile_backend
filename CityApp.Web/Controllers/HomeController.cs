using CityApp.Common.Caching;
using CityApp.Common.Models;
using CityApp.Data;
using CityApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Controllers
{
    public class HomeController : AccountBaseController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<HomeController>();

        private readonly CommonUserService _commonUserSvc;
        private AccountContext _accountCtx;

        public HomeController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache redisCache, IOptions<AppSettings> appSettings, AccountContext accountCtx, CommonUserService commonUserSvc)
            : base(commonContext, serviceProvider, redisCache, appSettings)
        {
            _commonUserSvc = commonUserSvc;
            _accountCtx = accountCtx;
        }

        public IActionResult Index()
        {
            //This is a citizen, send them to the citizen controller
            if (LoggedInUser.Permission == Data.Enums.SystemPermissions.None)
            {
                return RedirectToAction("Index", "Citizens");
            }

            //This is vendor, send them to the Vendor view. 
            if(LoggedInUser.Permission == Data.Enums.SystemPermissions.Vendor)
            {

            }

            //Only show metrics for these violation types. 
            var violationTypes = CommonAccount.ViolationTypes.ToList();

            var citations = _accountCtx.Citations
                .Include(m => m.Violation).ThenInclude(m => m.Category).ThenInclude(m => m.Type)
                .ForAccount(CommonAccount.Id).OrderByDescending(m => m.CreateUtc)
                .Where(m => m.Status == Data.Enums.CitationStatus.Open || m.Status == Data.Enums.CitationStatus.Approved || m.Status == Data.Enums.CitationStatus.InReview)
                .ToList();
            

            return View(citations);
        }
    }
}
