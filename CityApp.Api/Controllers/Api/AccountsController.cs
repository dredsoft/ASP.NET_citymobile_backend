using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using CityApp.Api.Models;
using CityApp.Data;
using CityApp.Common.Caching;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using CityApp.Common.Utilities;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using CityApp.Data.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using CityApp.Services;

namespace CityApp.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Accounts")]
    public class AccountsController : BaseApiController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<AccountsController>();
        private AccountContext _accountCtx;
        private readonly CommonAccountService _commonAccountSvc;
        private IHostingEnvironment _env;
        static public IConfigurationRoot Configuration { get; set; }


        public AccountsController(CommonContext commonContext, RedisCache cache, CommonAccountService commonAccountService, IOptions<AppSettings> appSettings, AccountContext accountCtx, IHostingEnvironment env) :
            base(commonContext, cache, appSettings)
        {
            _accountCtx = accountCtx;
            _env = env;
            _commonAccountSvc = commonAccountService;
        }

        [HttpPost("Nearest")]
        public async Task<IActionResult> Index([FromBody]AccountViewModel model, int limit = 5)
        {
            var response = new APIResponse<List<AccountViewModel>>() { Success = true };
            try
            {
                // Get appsettings.json file
                var builder = new ConfigurationBuilder()
                   .SetBasePath(_env.ContentRootPath)
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{_env.EnvironmentName}.json", optional: true)
                   .AddEnvironmentVariables();


                Configuration = builder.Build();


                //Fetch Default Connection string
                SqlConnection con = new SqlConnection(Configuration["Data:Common:ConnectionString"]);

                //Create IDb connection object and execute sql commond
                using (IDbConnection dbConnection = con)
                {
                    string sQuery = $"SELECT TOP {limit} CA.Id AS AccountId, CA.Name, C.Latitude, C.Longitude,CA.Number AS AccountNumber,"
                        + "SQRT( Power(69.1 * (C.Latitude - @startLatitude), 2) "
                        + " + Power(69.1 * (C.Longitude - @startLongitude) * COS(C.Latitude / 57.3), 2)) "
                        + " AS Distance FROM CommonAccounts CA LEFT JOIN Cities C ON CA.CityId = C.Id WHERE CA.Name != 'MASTER' AND CA.AllowPublicRegistration = 1 ORDER BY Distance";



                    dbConnection.Open();
                    var accountDetail =  dbConnection.QueryAsync<AccountViewModel>(sQuery, new { startLatitude = model.Latitude, startLongitude = model.Longitude }).Result.ToList();

                    //Filter out the accounts that have a distance greater that AppSettings.AccountMaxDistanceMiles
                    accountDetail = accountDetail.Where(m => m.Distance < AppSettings.AccountMaxDistanceMiles).ToList();


                    //Get Master Account
                    var master = await CommonContext.CommonAccounts.Include(m => m.City).Where(m => m.Name == "MASTER").Select(m => new AccountViewModel { Name = m.Name, AccountId = m.Id, Latitude = m.City.Latitude, Longitude= m.City.Longitude, AccountNumber = m.Number }).ToListAsync();
                    if(master.Any())
                    {
                        master.First().Name = "Current Location";
                    }

                    accountDetail.AddRange(master);

                    response.Success = true;
                    response.Message = $"Nearest accounts within {AppSettings.AccountMaxDistanceMiles} miles.";
                    response.Data = accountDetail;

                }
            }
            catch (Exception ex)
            {

                response.Success = false;
                response.Errors.Add(new Error { Code = 0, Message = ex.Message });
            }
            return Ok(response);


        }


        [HttpPost("All")]
        public async Task<IActionResult> All([FromBody]AccountViewModel model, int limit = 5)
        {
            var response = new APIResponse<List<AccountViewModel>>() { Success = true };
            try
            {
                var account = await CommonContext.CommonAccounts.ToListAsync();

                response.Success = true;
                response.Message = $"Nearest accounts within {AppSettings.AccountMaxDistanceMiles} miles.";
                response.Data = Mapper.Map<List<AccountViewModel>>(account);

            }
            catch (Exception ex)
            {

                response.Success = false;
                response.Errors.Add(new Error { Code = 0, Message = ex.Message });
            }
            return Ok(response);


        }



        [HttpPost("AssociateUser")]
        public async Task<IActionResult> AssociateUser([FromBody]AccountUserAssociationModel model)
        {
            var response = new APIResponse<CommonUserAccountModel>() { Success = true };

            var commonUser = await CommonContext.Users.SingleOrDefaultAsync(m => m.Id == model.UserId);

            if (commonUser == null)
            {
                response.Success = false;
                response.Message = "User could not be found";
            }
            else
            {
                var commonUserAccount = await CommonContext.UserAccounts.Where(m => m.UserId == model.UserId && m.AccountId == model.AccountId).SingleOrDefaultAsync();

                //If these is no association, create it. 
                if (commonUserAccount == null)
                {
                    commonUserAccount = new CommonUserAccount { AccountId = model.AccountId, UserId = model.UserId, CreateUserId = model.UserId, UpdateUserId = model.UserId };

                    CommonContext.UserAccounts.Add(commonUserAccount);
                    await CommonContext.SaveChangesAsync();
                }

                await _commonAccountSvc.SaveAccountUser(commonUser, commonUser.CreateUserId);

                response.Data = Mapper.Map<CommonUserAccountModel>(commonUserAccount);
            }

            return Ok(response);

        }
    }
}
