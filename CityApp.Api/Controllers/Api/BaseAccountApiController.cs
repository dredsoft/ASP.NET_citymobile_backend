using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using CityApp.Api.Filters;
using CityApp.Common.Models;
using CityApp.Api.Models;
using CityApp.Data;
using CityApp.Common.Caching;
using Microsoft.Extensions.Options;

namespace CityApp.Api.Controllers
{
    [AccountActionFilter]
    public class BaseAccountApiController : BaseApiController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<BaseAccountApiController>();

        public CachedAccount CommonAccount { get; set; }

        public CommonUserAccountModel CommonUserAccount { get; set; }

        public BaseAccountApiController(CommonContext commonContext, RedisCache cache, IOptions<AppSettings> appSettings) :
            base(commonContext, cache, appSettings)

        {

        }
    }

}
