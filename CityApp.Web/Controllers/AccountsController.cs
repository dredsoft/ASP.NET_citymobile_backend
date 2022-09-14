using System;
using CityApp.Data;
using Serilog;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using CityApp.Common.Caching;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using CityApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using CityApp.Web.Models.Accounts;
using CityApp.Data.Models;
using CityApp.Services;
using CityApp.Common.Utilities;

namespace CityApp.Web.Controllers
{
    [Authorize]
    public class AccountsController : BaseController
    {
        private static readonly ILogger _logger = Log.ForContext<AccountBaseController>();

        public AccountsController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache cache, IOptions<AppSettings> appSettings)
            :base(commonContext, serviceProvider, cache, appSettings)
        {

        }

        public async Task<IActionResult> Index()
        {

            //This is a citizen, send them to the citizen controller
            if (LoggedInUser.Permission == Data.Enums.SystemPermissions.None)
            {
                return RedirectToAction("Index", "Citizens");
            }

            //This is vendor, send them to the Vendor view. 
            if (LoggedInUser.Permission == Data.Enums.SystemPermissions.Vendor)
            {

            }

            //get all accounts for this user
            var accounts = await CommonContext.UserAccounts.AsNoTracking()
                .Include(m => m.Account).ThenInclude(m => m.City)
                .Where(m => m.UserId == LoggedInUser.Id).ToListAsync();

            var model = new AccountList();
            model.Accounts = Mapper.Map<List<AccountListItem>>(accounts.Select(m => m.Account));

            if(model.Accounts.Count == 1)
            {
                return RedirectToAction("Index","Home", new {AccountNum = model.Accounts.First().Number });
            }

            return View(model);
        }

        public  IActionResult NotAuthorized()
        {
            return View();

        }

        [AllowAnonymous]
        public async Task <IActionResult> Info (long id, Guid? eid = null)
        {
            var account = await CommonContext.CommonAccounts.Include(m => m.Partition).SingleOrDefaultAsync(m => m.Number == id);

            var model = new AccountInfoViewModel() ;

            if(account != null)
            {
                model = await GetSampleAccountInfoViewModel(account, eid);
            }

            return View(model);
        }

        private async Task<AccountInfoViewModel> GetSampleAccountInfoViewModel(CommonAccount account, Guid? eventId)
        {
            var model = new AccountInfoViewModel() { Account = account };

            var _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(account.Partition.ConnectionString));

            var events = _accountCtx.Events.ForAccount(account.Id).AsNoTracking();

            if(eventId.HasValue)
            {
                events = events.Where(m => m.Id == eventId.Value);
            }

            model.Events = Mapper.Map<List<AccountEvent>>(await events.OrderByDescending(m => m.CreateUtc).ToListAsync());

            return model;
        }
    }
}
