using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using CityApp.Common.Caching;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using CityApp.Data;
using System.Threading.Tasks;
using CityApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace CityApp.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{accountNum:long}/Violations")]
    public class ViolationsController : BaseAccountApiController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<ViolationsController>();
        private AccountContext _accountCtx;

        public ViolationsController(CommonContext commonContext, RedisCache cache, IOptions<AppSettings> appSettings, AccountContext accountCtx) :
            base(commonContext, cache, appSettings)
        {
            _accountCtx = accountCtx;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            ViolationsModel model = new ViolationsModel();
            //get commom violation type
            var vioTypeId = await CommonContext.CommonAccountViolationTypes.AsNoTracking()
                                .Where(s => s.AccountId.Equals(CommonAccount.Id)).Select(m => m.ViolationTypeId).ToListAsync();

            //get Violation that are disabled or that is related to passed accountid
            var violations = _accountCtx.Violations.Include(m => m.Questions).ThenInclude(m => m.CreateUser)
                .Include(m => m.Category)
                .Include(m => m.Category.Type)
                .ForAccount(CommonAccount.Id)
                .Where(m => !m.Disabled)
                .AsQueryable();


            //Filter the violation by the types that this account is associated with.
            violations = violations.Where(m => vioTypeId.Contains(m.Category.Type.CommonViolationTypeId));

            //Map Model with entity
            model.Violation = Mapper.Map<List<ViolationsListModel>>(violations);

            var responseModel = new APIResponse<List<ViolationsListModel>>()
            {
                Data = model.Violation
            };

            return Ok(responseModel);
        }


    }
}
