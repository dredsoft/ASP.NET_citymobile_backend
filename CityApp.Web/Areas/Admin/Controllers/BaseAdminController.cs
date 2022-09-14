using System;
using Microsoft.AspNetCore.Mvc;
using CityApp.Data;
using Serilog;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using Microsoft.AspNetCore.Authorization;
using CityApp.Web.Constants;
using CityApp.Common.Caching;
using AutoMapper;
using CityApp.Data.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using CityApp.Web.Controllers;
using CityApp.Web.Filters;

namespace CityApp.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = SystemPermission.Administrator)]
    [Area(Area.Admin)]   
    public class BaseAdminController : BaseController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<BaseAdminController>();


        public BaseAdminController(CommonContext commonContext, RedisCache redisCache, IServiceProvider serviceProvider, IOptions<AppSettings> appSettings) 
            : base(commonContext, serviceProvider,redisCache,appSettings)
        {
        }

    }
}
