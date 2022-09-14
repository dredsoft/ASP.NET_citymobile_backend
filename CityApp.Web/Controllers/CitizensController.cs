using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using CityApp.Services;
using CityApp.Data;
using CityApp.Common.Caching;
using CityApp.Common.Models;
using Microsoft.Extensions.Options;
using CityApp.Web.Models.Citations;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using CityApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using CityApp.Common.Extensions;
using CityApp.Web.Models;
using CityApp.Common.Utilities;
using CityApp.Web.Filters;
using CityApp.Data.Models;
using CityApp.Services.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.ComponentModel;
using CityApp.Data.Extensions;
using CityApp.Web.Constants;


namespace CityApp.Web.Controllers
{

    public class CitizensController : BaseController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<CitizensController>();
        private AccountContext _accountCtx;
        private CommonContext _commonContext;
        private readonly AppSettings _appSettings;

        public CitizensController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache redisCache, IOptions<AppSettings> appSettings, AccountContext accountCtx)
            : base(commonContext, serviceProvider, redisCache, appSettings)
        {
            _commonContext = commonContext;
            _appSettings = appSettings.Value;
            _accountCtx = accountCtx;
        }


        public async Task<IActionResult> Index()
        {


            //Get a list of Accounts for a user
            var userAccount = await CommonContext.UserAccounts.Include(m => m.Account).ThenInclude(m => m.Partition).Where(m => m.UserId == User.GetLoggedInUserId().Value).ToListAsync();

            var citations = new List<Citation>();

            //CitationListModel model = new CitationListModel();


            //Loop through each AccountContext for this user and get a list of Citations
            foreach (var commonAccount in userAccount)
            {
                //Get the correct account database based on the partition that was chosen for the account.
                var accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Account.Partition.ConnectionString));

                var citationsForAccount = accountCtx.Citations.Include(x => x.Account)
                        .Include(x => x.Violation)
                        .Include(x => x.AssignedTo)
                        .Include(m => m.Attachments).ThenInclude(m => m.Attachment)
                        .OrderByDescending(x => x.CreateUtc)
                        .Where(m => m.AccountId == commonAccount.AccountId)
                        .AsQueryable();

                citations.AddRange(citationsForAccount);
                // citations.AddRange(new CitationListModel { AccountId= citationsForAccount. });

            }

            CitationListViewModel model = new CitationListViewModel();
            model.CitationList = Mapper.Map<List<CitationListModel>>(citations);
            model.AccountList = Mapper.Map<List<AccountUserList>>(userAccount);
            return View(model);
        }




    }
}