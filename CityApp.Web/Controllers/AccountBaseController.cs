using System;
using CityApp.Data;
using Serilog;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using CityApp.Common.Caching;
using Microsoft.AspNetCore.Authorization;
using CityApp.Web.Models;
using CityApp.Web.Filters;

namespace CityApp.Web.Controllers
{
    [Authorize]
    [AccountActionFilter]
    public class AccountBaseController : BaseController
    {
        private static readonly ILogger _logger = Log.ForContext<AccountBaseController>();

        public CachedAccount CommonAccount { get; set; }

        public CommonUserAccountModel CommonUserAccount { get; set; }

        public AccountBaseController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache cache, IOptions<AppSettings> appSettings)
            :base(commonContext, serviceProvider, cache, appSettings)
        {

        }
    }
}
