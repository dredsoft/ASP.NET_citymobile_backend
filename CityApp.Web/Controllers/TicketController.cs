using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CityApp.Data;
using Serilog;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using Microsoft.EntityFrameworkCore;
using CityApp.Common.Caching;
using CityApp.Services;
using CityApp.Common.Utilities;
using CityApp.Web.Models.Ticket;
using Microsoft.AspNetCore.DataProtection;
using CityApp.Data.Models;
using AutoMapper;
using CityApp.Web.Models.Citations;
using CityApp.Web.Models;
using CityApp.Services.Models;
using CityApp.Common.Extensions;
using CityApp.Data.Enums;

namespace CityApp.Web.Controllers
{

    /// <summary>
    /// The Ticket Controller is for External users.
    /// This users are violators who recieved the ticket. 
    /// </summary>
    public class TicketController : BaseController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<TicketController>();
        private readonly IDataProtector _dataProtector;
        private readonly FileService _fileService;
        private readonly CommonAccountService _commonAccountSvc;
        private readonly StripeService _stripeSvc;
        private AccountContext _accountCtx;
        private readonly CitationService _citationSvc;


        public TicketController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache redisCache, IOptions<AppSettings> appSettings,  CommonAccountService commonAccountSvc, FileService fileService, IDataProtectionProvider dataProtector, StripeService stripeSvc, CitationService citationSvc)
            : base(commonContext, serviceProvider, redisCache, appSettings)
        {
            _commonAccountSvc = commonAccountSvc;
            _dataProtector = dataProtector.CreateProtector(GetType().FullName);
            _fileService = fileService;
            _stripeSvc = stripeSvc;
            _citationSvc = citationSvc;
        }

        public IActionResult Find()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Find(FindTicketViewModel model)
        {
            if(ModelState.IsValid)
            {
                var accountDetail = await CommonContext.CommonAccounts
                    .Include(m =>  m.Partition)
                    .Where(account => account.Number == model.AccountNumber)
                    .SingleOrDefaultAsync();

                //Check if this account exists
                if (accountDetail == null)
                {
                    ModelState.AddModelError("AccountNumber", "Invalid Account Number");
                    return View(model);
                }
                else
                {
                    var _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));

                    //Find violation
                    var license = model.LicensePlate.Trim();
                    var state = model.State.Trim();

                    var citation = await _accountCtx.Citations.ForAccount(accountDetail.Id)
                        .Where(m => m.CitationNumber == model.CitationNumber)
                        .Where(m => m.LicenseState == state)
                        .Where(m => m.LicensePlate == license)
                        .OrderByDescending(m => m.CreateUtc)
                        .FirstOrDefaultAsync();

                    if(citation == null)
                    {
                        ModelState.AddModelError("", "Cannot find Ticket.  Please call 555-555-5555 for assitance");
                    }
                    else
                    {
                        //Encrypt CitationID and Send user to Citation Page
                        var encryptedCitationId = _dataProtector.Protect( $"{accountDetail.Id}&{citation.Id.ToString()}");
                        return RedirectToAction("Citation", new { id = encryptedCitationId });
                    }
                }

            }

            return View(model);
        }

        /// <summary>
        /// Citation Detail 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Citation(string id)
        {
            ViewData["protectedID"] = id;
            //{accountId}-{violationId}
            var decryptedString = _dataProtector.Unprotect(id).Split('&');
            var accountId = Guid.Parse(decryptedString[0]);
            var citationId = Guid.Parse(decryptedString[1]);

            var citation = await GetCitation(accountId, citationId);

            if(citation != null)
            {
                var citationViewModel = Mapper.Map<CitationViolationListItem>(citation);

                ApplyVideoAndImageUrl(citation, citationViewModel, _fileService);

                return View(citationViewModel);
            }

            return RedirectToAction("Find");
        }

        /// <summary>
        /// View Ticket Payment Page or warning Payment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Pay(string id, bool isWarning = false)
        {
            var test = _dataProtector.Unprotect(id);
            //{accountId}-{violationId}
            var decryptedString = _dataProtector.Unprotect(id).Split('&');
            var accountId = Guid.Parse(decryptedString[0]);
            var citationId = Guid.Parse(decryptedString[1]);

            var citation = await GetCitation(accountId, citationId);

            if (citation != null)
            {
                var citationFine = citation.Balance.HasValue? citation.Balance : citation.FineAmount;
                var amount = (int)((citationFine + AppSettings.ProcessingFee) * 100);
                var model = new TicketPaymentModel {AccountId = accountId, CitationId = citation.Id, AmountDue = amount };

                return View(model);
            }

            return RedirectToAction("Find");
        }

        /// <summary>
        /// Process Payment
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult>Pay(TicketPaymentModel model)
        {
            var account = await CommonContext.CommonAccounts.Include(m => m.Partition).Where(m => m.Id == model.AccountId).SingleAsync();
            var citation = await GetCitation(account.Id, model.CitationId);

            //Create Stripe Charge
            var stripeCharge = await _stripeSvc.ChargeCardAsync(new CreditCardModel {
                Amount = model.AmountDue,
                SourceToken = model.StripeToken
            }, citation, account, ChargeTypeEnum.PayCitation);


            //Record charge
            var payment = new CitationPayment {
                AccountId = account.Id,
                CitationId = citation.Id,
                CreateUserId = citation.CreateUserId, 
                UpdateUserId = citation.CreateUserId,
                Status = PaymentType.Stripe,
                ProcessingFee = AppSettings.ProcessingFee,
                CitationFineAmount = citation.Balance.HasValue ? citation.Balance.Value : 0f,
                ChargeAmount = model.AmountDue,
                ChargeId = stripeCharge.Id };

            _accountCtx.CitationPayments.Add(payment);

            citation.Balance = citation.Payments.Sum(m => m.ChargeAmount) + payment.ChargeAmount;


            await _accountCtx.SaveChangesAsync();

            await _citationSvc.CreateAuditEvent(account.Id, payment.CitationId, $"{string.Format("{0:C}", model.AmountDue.PenniesToDollarAmount())} payment charged", payment.CreateUserId, Data.Enums.CitationAuditEvent.CitationSuccessPayment);

            return RedirectToAction("PaymentComplete");
        }


        public async Task<IActionResult> PaymentComplete()
        {
            return View();
        }


        public async Task<IActionResult> WarningQuiz(string id)
        {
            var test = _dataProtector.Unprotect(id);
            //{accountId}-{violationId}
            var decryptedString = _dataProtector.Unprotect(id).Split('&');
            var accountId = Guid.Parse(decryptedString[0]);
            var citationId = Guid.Parse(decryptedString[1]);

            var citation = await GetCitation(accountId, citationId);

            if (citation != null)
            {


                return Redirect($"https://govappsolutions.typeform.com/to/n5MRBy?accountid={citation.AccountId}&citationid={citation.Id}");
            }
            return View();
        }

        public async Task<IActionResult> WarningQuizComplete(string id)
        {
            var test = _dataProtector.Unprotect(id);
            //{accountId}-{violationId}
            var decryptedString = _dataProtector.Unprotect(id).Split('&');
            var accountId = Guid.Parse(decryptedString[0]);
            var citationId = Guid.Parse(decryptedString[1]);

            var citation = await GetCitation(accountId, citationId);

            if (citation != null)
            {


                return View();
            }
            return View();
        }


        #region Private

        private async Task<Citation> GetCitation(Guid accountId, Guid citationId)
        {
            var accountDetail = await CommonContext.CommonAccounts.Include(m => m.Partition).SingleOrDefaultAsync(account => account.Id == accountId);
            _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));
            var citaion = await _accountCtx.Citations.ForAccount(accountId)
                .Include(m => m.Attachments).ThenInclude(m => m.Attachment)
                .Include(m => m.Payments)
                .Where(m => m.Id == citationId).SingleOrDefaultAsync();

            return citaion;
        }

        #endregion
    }
}
