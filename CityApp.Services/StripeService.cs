using AutoMapper;
using CityApp.Common.Extensions;
using CityApp.Common.Models;
using CityApp.Common.Utilities;
using CityApp.Data;
using CityApp.Data.Enums;
using CityApp.Data.Extensions;
using CityApp.Data.Models;
using CityApp.Services.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Services
{
    public class StripeService : ICustomService
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<StripeService>();
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _env;
        private readonly CommonContext _commonCtx;
        private readonly CitationService _citationSvc;

        public StripeService(CommonContext commonCtx, IOptions<AppSettings> stripeSettings, CitationService citationSvc, IHostingEnvironment env,  IOptions<AppSettings> appSettings, IMapper mapper)
        {
            _commonCtx = commonCtx;
            _env = env;
            _appSettings = appSettings.Value;
            _mapper = mapper;
            _citationSvc = citationSvc;

            StripeConfiguration.SetApiKey(_appSettings.StripeSecretKey);
        }

        /// <summary>
        /// Create Charge with customer
        /// </summary>
        /// <param name="creditCard"></param>
        /// <returns></returns>
        public async Task<StripeCharge> ChargeCardAsync(CreditCardModel creditCard, Citation citation, CommonAccount account, ChargeTypeEnum chargeType)
        {
            Check.NotNull(creditCard, nameof(creditCard));

            var metaData = new Dictionary<string, string>();
            metaData["Account"] = account.Name;
            metaData["AccountNumber"] = account.Number.ToString();
            metaData["CitationNumber"] = citation.CitationNumber.ToString();
            metaData["Type"] = chargeType.GetDescription();
            // Token is created using Checkout or Elements!
            // Get the payment token submitted by the form:
            var token = creditCard.SourceToken; // Using ASP.NET MVC

            var options = new StripeChargeCreateOptions
            {
                Amount = creditCard.Amount,
                Currency = "usd",
                Description = chargeType.GetDescription(),
                SourceTokenOrExistingSourceId = token,
                Metadata = metaData
            };
            var service = new StripeChargeService();
            StripeCharge charge = await service.CreateAsync(options);

            return charge;
        }


        /// <summary>
        /// Refunds the payment.  Adds a negative payment record to the Payments table against this citation
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="accountNumber"></param>
        /// <param name="amountRefunded"></param>
        /// <returns></returns>
        public async Task RefundPayment(string paymentId, long accountNumber, int amountRefunded)
        {
            //Get Account Record
            var commonAccount = _commonCtx.CommonAccounts.Include(m => m.Partition).AsNoTracking().Where(m => m.Number == accountNumber).SingleOrDefault();

            if (commonAccount != null)
            {
                var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString));

                var payment = await accountContext.CitationPayments.Include(m => m.Citation).Where(m => m.ChargeId.Contains(paymentId)).FirstOrDefaultAsync();
                if (payment != null)
                {
                    payment.ChargeAmount = payment.ChargeAmount - amountRefunded;

                    var fineAmountRefunded = amountRefunded.PenniesToDollarAmount() - payment.ProcessingFee;

                    payment.Citation.Balance = payment.Citation.Balance - fineAmountRefunded;

                    await _citationSvc.CreateAuditEvent(commonAccount.Id, payment.CitationId, $"{string.Format("{0:C}",amountRefunded.PenniesToDollarAmount())} payment refunded", payment.CreateUserId, Data.Enums.CitationAuditEvent.CitationRefundPayment);
                }

                try
                {
                    accountContext.SaveChanges();
                }
                catch (Exception ex)

                {
                    _logger.Error(ex, $"Error trying to create Citation Refund Payment");
                }
            }

        }
    }
}
