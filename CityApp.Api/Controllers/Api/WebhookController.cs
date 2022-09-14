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
using CityApp.Data.Enums;
using CityApp.Api.Filters;
using System;
using CityApp.Services;
using CityApp.Common.Utilities;
using CityApp.Data.Models;
using Stripe;
using CityApp.Common.Serialization;
using CityApp.Api.Models.WebHooks;
using Newtonsoft.Json;
using CityApp.Services.Models;
using System.IO;
using System.Text;

namespace CityApp.Api.Controllers
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Webhook/")]
    public class WebhookController : BaseApiController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<WebhookController>();
        private AccountContext _accountCtx;
        private CityApp.Services.StripeService _stripeSvc;
        private readonly WarningQuizService _warningQuizSvc;


        public WebhookController(CommonContext commonContext, RedisCache cache, IOptions<AppSettings> appSettings, CityApp.Services.StripeService stripeSvc, WarningQuizService warningQuizSvc) :
            base(commonContext, cache, appSettings)
        {
            _stripeSvc = stripeSvc;
            _warningQuizSvc = warningQuizSvc;
        }


        [HttpPut]
        [Route("{accountNum:long}/Citation/{citationNumber}")]
        public async Task<IActionResult> Update([FromBody]CitationStatus status, long accountNum, long citationNumber)
        {
            var commonAccount = await CommonContext.CommonAccounts.AsNoTracking()
                .Include(m => m.Partition)
                .Where(m => m.Number == accountNum)
                .SingleOrDefaultAsync();

            if(commonAccount == null)
            {
                _logger.Error($"CommonAccount is null.  Could not find accountNum:{accountNum}");

                return ErrorResult(Common.Enums.ErrorCode.PreconditionFailed, "Invalid request");
            }

            var accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString));
            if(accountCtx == null)
            {
                _logger.Error($"Could not load AccountCtx for accountNum:{accountNum}");

                return ErrorResult(Common.Enums.ErrorCode.Server, "Cannot load account context.");
            }

            var citation = await accountCtx.Citations.ForAccount(commonAccount.Id).Where(m => m.CitationNumber == citationNumber).SingleOrDefaultAsync();
            if (citation == null)
            {
                _logger.Error($"Citation.Id:{citationNumber} cannot be found.");

                return ErrorResult(Common.Enums.ErrorCode.PreconditionFailed, "Citation cannot be found for this account.");
            }


            //Only change status if it's different. 
            if (status != citation.Status)
            {
                var oldStatus = citation.Status.ToString();

                citation.Status = status;
                citation.UpdateUtc = DateTime.UtcNow;
                citation.Comments.Add(new CitationComment() { AccountId = commonAccount.Id, CitationId = citation.Id, IsPublic = false, Comment = $"Exteral system status change from {oldStatus} to {status.ToString()}", CreateUserId = citation.CreateUserId, UpdateUserId = citation.CreateUserId });

                await accountCtx.SaveChangesAsync();
                accountCtx.Dispose();
            }

            return Ok(new APIResponse<string>() {Message="Status updated." });
        }


        [Route("stripe")]
        [HttpPost]
        public async Task<IActionResult> Stripe([FromBody] StripeEvent stripeEvent)
        {
            var result = new StripeBaseCommand();

            try
            {
                switch (stripeEvent.Type)
                {
                    case StripeEvents.ChargeRefunded:
                        StripeRefundModel refund = StripeEventUtility.ParseEventDataItem<StripeRefundModel>(stripeEvent.Data.Object);
                        await _stripeSvc.RefundPayment(refund.id, refund.metadata.AccountNumber, refund.amount_refunded);
                        break;
                    case StripeEvents.CustomerSubscriptionUpdated:
                        
                        break;

                    case StripeEvents.InvoicePaymentFailed:  // Invoice is sent and payment results in failure.
                        //Undo Payment
                        break;
                }
            }
            catch (StripeException ex)
            {
                var message = $"Unhandled {nameof(StripeException)} caught in {nameof(WebhookController)}.{nameof(Stripe)}. StripeEvent.Type: {stripeEvent.Type}";
                _logger.Error(ex, message);
                result.Logs.Add(message);

                //await SendWebHookErrorEmail($"[Stripe Webhook Stripe Exception ({_env.EnvironmentName})]", stripeEvent, ex);
            }
            catch (Exception ex)
            {
                var message = $"Unhandled Exception caught in {nameof(WebhookController)}.{nameof(Stripe)}. StripeEvent.Type: {stripeEvent.Type}";
                _logger.Error(ex, message);
                result.Logs.Add(message);

                //await SendWebHookErrorEmail($"[Stripe Webhook System Exception ({_env.EnvironmentName})]", stripeEvent, ex);
            }

            return Ok(JsonNet.Serialize(new { result.Logs }, prettyPrint: true));
        }


        [Route("warningquiz")]
        [HttpPost]
        public async Task<IActionResult> Warningquiz()
        {
            var request = this.Request;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var content =  await reader.ReadToEndAsync();
                await _warningQuizSvc.CompleteWarningQuiz(content);
            }

            return Ok();
        }

    }
}
