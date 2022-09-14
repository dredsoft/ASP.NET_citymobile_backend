using CityApp.Common.Caching;
using CityApp.Common.Models;
using CityApp.Common.Utilities;
using CityApp.Data;
using CityApp.Data.Models;
using CityApp.Services.Models;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CityApp.Services
{
    /// <summary>
    /// Warning Quiz Related Tasks
    /// </summary>
    public class WarningQuizService : ICustomService
    {
        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        private static readonly object _randomLock = new object();
        private static readonly ILogger _logger = Log.Logger.ForContext<TicketWorkFlowService>();
        private IHostingEnvironment _env;
        private AppSettings _appSettings;
        private readonly CitationService _citationSvc;
        private readonly CommonContext _commonCtx;
        private CachedAccount _cachedAccount;


        public WarningQuizService(RedisCache cache, IHostingEnvironment environment, CommonContext commonCtx, IOptions<AppSettings> appSettings,CitationService citationSvc )
        {
            _env = environment;
            _commonCtx = commonCtx;
            _appSettings = appSettings.Value;
            _citationSvc = citationSvc;
        }


        /// <summary>
        /// Record that quiz has been completed.  Mark Citation as Closed.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public async Task CompleteWarningQuiz(string payload)
        {
            var warningQuizReponse = JsonConvert.DeserializeObject<TypeFormPayload>(payload);

            var accountDetail = await _commonCtx.CommonAccounts
                .Include(m => m.Partition)
                .Where(account => account.Id == warningQuizReponse.AccountId)
                .SingleOrDefaultAsync();

            var _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));


            var citation = await _accountCtx.Citations.ForAccount(accountDetail.Id)
                .Where(m => m.Id == warningQuizReponse.CitatoinId)
                .OrderByDescending(m => m.CreateUtc)
                .FirstOrDefaultAsync();

            //Close Citation
            citation.Status = Data.Enums.CitationStatus.Closed;
            citation.ClosedReason = "Warning Quiz Complete";

            //Add warning quiz record
            var warningQuiz = new WarningQuizResponse {AccountId = accountDetail.Id, CitationId = citation.Id, Payload = payload, CreateUserId = citation.CreateUserId, UpdateUserId = citation.CreateUserId};
            _accountCtx.WarningEventRespones.Add(warningQuiz);
            await _accountCtx.SaveChangesAsync();

            //Create Audit event
            _citationSvc.CreateAuditEvent(accountDetail.Id, citation.Id, "Quiz Completed", citation.CreateUserId, Data.Enums.CitationAuditEvent.QuizComplete);

        }


    }
}
