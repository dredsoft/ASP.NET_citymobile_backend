using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using CityApp.Api.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using CityApp.Data;
using CityApp.Common.Caching;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using CityApp.Common.Extensions;
using CityApp.Services;
using CityApp.Api.Models.Citation;
using CityApp.Common.Utilities;
using CityApp.Data.Models;
using CityApp.Api.Models.Common;

namespace CityApp.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Common")]
    public class CommonController : BaseApiController
    {
        private readonly CommonAccountService _commonAccountSvc;
        private readonly CommonUserService _commonUserSvc;
        private AccountContext _accountCtx;
        private readonly MailService _mailSvc;
        private static readonly ILogger _logger = Log.Logger.ForContext<CitationsController>();

        public CommonController(CommonContext commonContext, RedisCache cache, IOptions<AppSettings> appSettings, CommonAccountService commonAccountSvc, CommonUserService commonUserSvc, AccountContext accountCtx, MailService mailSvc) :
            base(commonContext, cache, appSettings)
        {
            _accountCtx = accountCtx;
            _commonAccountSvc = commonAccountSvc;
            _commonUserSvc = commonUserSvc;
            _mailSvc = mailSvc;
        }

        /// <summary>
        /// Get AccountUser Information for a specific user. 
        /// Only System Admin can get data for other users
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("SupportEmail")]
        [HttpPost]
        public async Task<IActionResult> SupportEmail([FromBody]MailModel model)
        {

            var response = new APIResponse<bool>();

            var body = $"Name: {model.Name} ({model.UserId}) <br /> Email: {model.From} <br /> Phone: {model.Phone} <br /> <br /> Message: <br /> {model.Message} <br /> <br /> Build: {model.BuildVersion}<br />Created: {model.Created}";

            response.Data = await _mailSvc.SendEmailAsync("support@texttoticket.com", model.Subject, body);

            return Ok(response);
        }
    }
}
