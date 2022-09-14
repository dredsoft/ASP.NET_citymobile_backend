using CityApp.Services.Models;
using Microsoft.EntityFrameworkCore;
using CityApp.Data;
using CityApp.Data.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CityApp.Common.Caching;
using CityApp.Common.Models;
using AutoMapper;
using CityApp.Data.Enums;
using CityApp.Common.Utilities;

namespace CityApp.Services
{
    /// <summary>
    /// This service is responsible for getting citation metrics for the dashboard. 
    /// </summary>
    public class AccountMetricsService : ICustomService
    {

        private static readonly ILogger _logger = Log.Logger.ForContext<AccountMetricsService>();

        private static readonly TimeSpan _dashboardExpiry = TimeSpan.FromHours(24);
        private static readonly TimeSpan _dashboardChartExpiry = TimeSpan.FromHours(4);

        public AccountMetricsService()
        {

        }


    }
}
