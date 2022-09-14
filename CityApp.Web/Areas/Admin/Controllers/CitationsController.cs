using System;
using CityApp.Common.Caching;
using CityApp.Common.Models;
using CityApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using CityApp.Services;
using Microsoft.AspNetCore.Authorization;
using CityApp.Web.Constants;
using System.Threading.Tasks;
using CityApp.Areas.Admin.Models;
using CityApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using CityApp.Web.Areas.Admin.Models;
using CityApp.Web.Models.Citations;
using System.Collections.Generic;
using AutoMapper;
using Humanizer;
using CityApp.Common.Extensions;
using CityApp.Common.Utilities;
using CityApp.Web.Models;

namespace CityApp.Web.Areas.Admin.Controllers
{
    public class CitationsController : BaseAdminController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<ViolationsController>();
        private readonly CommonAccountService _commonAccountSvc;
        private readonly CitationService _citationSvc;
        private readonly FileService _fileSvc;
        private AccountContext _accountCtx;
        private ViolationService _violationSvc;



        public CitationsController(CommonContext commonContext, RedisCache redisCache, IServiceProvider serviceProvider, IOptions<AppSettings> appSettings, CommonAccountService commonAccountSvc, CitationService citationSvc, FileService fileSvc)
            : base(commonContext, redisCache, serviceProvider, appSettings)
        {
            _commonAccountSvc = commonAccountSvc;
            _citationSvc = citationSvc;
            _fileSvc = fileSvc;
        }


        #region Citations

        /// <summary>
        /// getting all citation list
        /// </summary>
        /// <param name="CreatedFrom"></param>
        /// <param name="Createdto"></param>
        /// <param name="StatusId"></param>
        /// <param name="AssignedToId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(AdminCitationListViewModel model)
        {
             var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            if (model.AccountId.HasValue)
            {
                var accountDetail = await CommonContext.CommonAccounts.Include(m => m.Partition).SingleOrDefaultAsync(account => account.Id == model.AccountId);
                _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));

                var citations = ApplyCitationFilter(model);

                #region sort
                switch (model.SortOrder)
                {
                    case "Status":
                        if (model.SortDirection == "DESC")
                        {
                            citations = citations.OrderByDescending(s => s.Status);
                        }
                        else
                        {
                            citations = citations.OrderBy(s => s.Status);
                        }
                        break;
                    case "Violation":
                        if (model.SortDirection == "DESC")
                        {
                            citations = citations.OrderByDescending(s => s.Violation.Name);
                        }
                        else
                        {
                            citations = citations.OrderBy(s => s.Violation.Name);
                        }
                        break;
                    case "AssignedTo":
                        if (model.SortDirection == "DESC")
                        {
                            citations = citations.OrderByDescending(s => s.AssignedTo);
                        }
                        else
                        {
                            citations = citations.OrderBy(s => s.AssignedTo);
                        }
                        break;
                    case "Created":
                        if (model.SortDirection == "DESC")
                        {
                            citations = citations.OrderByDescending(s => s.CreateUtc);
                        }
                        else
                        {
                            citations = citations.OrderBy(s => s.CreateUtc);
                        }
                        break;
                    case "License":
                        if (model.SortDirection == "DESC")
                        {
                            citations = citations.OrderByDescending(s => s.LicensePlate);
                        }
                        else
                        {
                            citations = citations.OrderBy(s => s.LicensePlate);
                        }
                        break;
                    case "Number":
                        if (model.SortDirection == "DESC")
                        {
                            citations = citations.OrderByDescending(s => s.CitationNumber);
                        }
                        else
                        {
                            citations = citations.OrderBy(s => s.CitationNumber);
                        }
                        break;
                    default:
                        if (model.SortDirection == "DESC")
                        {
                            citations = citations.OrderByDescending(s => s.CreateUtc);
                        }
                        else
                        {
                            citations = citations.OrderBy(s => s.CreateUtc);
                        }
                        break;
                }
                #endregion

                var totalQueryCount = await citations.CountAsync();
                model.CitationsListItem = Mapper.Map<List<CitationsListItem>>(await citations.Skip(offset).Take(model.PageSize).ToListAsync());

                foreach (var m in model.CitationsListItem)
                {
                    m.CreatedHumanizerDate = m.Created.Humanize();
                }

                model.Paging.CurrentPage = currentPageNum;
                model.Paging.ItemsPerPage = model.PageSize;
                model.Paging.TotalItems = totalQueryCount;
            }

            #region dropdown lists

            //Accounts List
            //TODO: this should be cached and retrieved from a service
            var accounts = await CommonContext.CommonAccounts.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString(),
            })
            .ToListAsync();           

            model.Accounts = accounts;

            #endregion


            return View(model);
        }

        /// <summary>
        /// Citation Detail 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        public async Task<IActionResult> Citation(Guid id, Guid accountId)
        {
            var accountDetail = await CommonContext.CommonAccounts.Include(m => m.Partition).SingleOrDefaultAsync(account => account.Id == accountId);
            _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));


            var citation = await GetCitation(accountId, id);

            if (citation != null)
            {
                var citationViewModel = Mapper.Map<CitationViolationListItem>(citation);

                ApplyVideoAndImageUrl(citation, citationViewModel, _fileSvc);

                return View(citationViewModel);
            }

            return RedirectToAction("Index");
        }

        #endregion


        #region Payments

        public async Task<IActionResult> CreatePayment(Guid citationId, Guid accountId)
        {
            var accountDetail = await CommonContext.CommonAccounts.Include(m => m.Partition).SingleOrDefaultAsync(account => account.Id == accountId);
            _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));

            var citation = await GetCitation(accountId, citationId);

            var model = new PaymentViewModel {CitationId = citationId, AccountId = accountId, CitationFineAmount = citation.Balance.HasValue? citation.Balance.Value : 0f };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(PaymentViewModel model)
        {

            ValidateCreatePayment(model);

            if(ModelState.IsValid)
            {
                var accountDetail = await CommonContext.CommonAccounts.Include(m => m.Partition).SingleOrDefaultAsync(account => account.Id == model.AccountId);
                _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));

                var citation = await GetCitation(model.AccountId, model.CitationId);
                var citationPayment = Mapper.Map<CitationPayment>(model);
                citationPayment.AccountId = accountDetail.Id;
                citationPayment.CreateUtc = DateTime.UtcNow;
                citationPayment.UpdateUtc = DateTime.UtcNow;
                citationPayment.CreateUserId = citation.CreateUserId;
                citationPayment.UpdateUserId = citation.CreateUserId;

                citation.Payments.Add(citationPayment);

                //Calcualte the new balance.
                citation.Balance = citation.Payments.Select(m => m.ChargeAmount).Sum();

                await _accountCtx.SaveChangesAsync();
                return RedirectToAction("Citation", new { id = model.CitationId, accountId = model.AccountId });
            }

            return View(model);
        }

        private void ValidateCreatePayment(PaymentViewModel model)
        {
            if(model.ChargeAmount == 0f)
            {
                ModelState.AddModelError("ChargeAmount", "Cannot be 0");
            }

            if (model.CitationFineAmount == 0f)
            {
                ModelState.AddModelError("CitationFineAmount", "Cannot be 0");
            }
        }

        public async Task<IActionResult> EditPayment(Guid id, Guid accountId)
        {
            var accountDetail = await CommonContext.CommonAccounts.Include(m => m.Partition).SingleOrDefaultAsync(account => account.Id == accountId);
            _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));

            var payment = await _accountCtx.CitationPayments.Where(m => m.Id == id).SingleAsync();

            var model = Mapper.Map<PaymentViewModel>(payment);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPayment(PaymentViewModel model)
        {

            if (ModelState.IsValid)
            {
                var accountDetail = await CommonContext.CommonAccounts.Include(m => m.Partition).SingleOrDefaultAsync(account => account.Id == model.AccountId);
                _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));

                var payment = await _accountCtx.CitationPayments.Include(m => m.Citation).ThenInclude(m => m.Payments).Where(m => m.Id == model.Id).SingleAsync();

                payment.ChargeAmount = model.ChargeAmount;
                payment.CitationFineAmount = model.CitationFineAmount;
                payment.ProcessingFee = model.ProcessingFee;
                payment.Status = model.Status;
                payment.ChargeId = model.ChargeId;

                payment.Citation.Balance = payment.Citation.Payments.Select(m => m.ChargeAmount).Sum();

                await _accountCtx.SaveChangesAsync();

                return RedirectToAction("Citation", new { id = model.CitationId, accountId = model.AccountId });
            }

            return View(model);
        }

        #endregion

        #region Private 
        private async Task<Citation> GetCitation(Guid accountId, Guid citationId)
        {
            var citaion = await _accountCtx.Citations.ForAccount(accountId)
                .Include(m => m.Attachments).ThenInclude(m => m.Attachment)
                .Include(m => m.Payments)
                .Where(m => m.Id == citationId).SingleOrDefaultAsync();

            return citaion;
        }

        private IQueryable<Citation> ApplyCitationFilter(AdminCitationListViewModel model)
        {
            var citations = _accountCtx.Citations.ForAccount(model.AccountId.Value)
                           .Include(x => x.Violation).ThenInclude(m => m.Category).ThenInclude(m => m.Type)
                           .Include(x => x.Payments)
                           .OrderByDescending(x => x.CreateUtc)
                           .AsQueryable();


            if (model.CreatedFrom.HasValue)
            {
                citations = citations.Where(s => s.CreateUtc >= model.CreatedFrom.Value.Floor());
            }
            if (model.CreatedTo.HasValue)
            {
                citations = citations.Where(s => s.CreateUtc <= model.CreatedTo.Value.Ceiling());
            }
            if (model.StatusId.HasValue)
            {
                citations = citations.Where(s => s.Status == model.StatusId);
            }
            if (model.AssignedToId.HasValue)
            {
                citations = citations.Where(s => s.AssignedToId == model.AssignedToId);
            }
            if (!string.IsNullOrWhiteSpace(model.LicensePlate))
            {
                citations = citations.Where(s => s.LicensePlate.Contains(model.LicensePlate));
            }
            if (model.ViolationTypeId.HasValue)
            {
                citations = citations.Where(s => s.Violation.Category.TypeId == model.ViolationTypeId);
            }
            if (model.ViolationId.HasValue)
            {
                citations = citations.Where(s => s.ViolationId == model.ViolationId);
            }
            if (!string.IsNullOrEmpty(model.PostalCode))
            {
                citations = citations.Where(s => s.PostalCode.Contains(model.PostalCode));
            }
            if (!string.IsNullOrEmpty(model.Street))
            {
                citations = citations.Where(s => s.Street.Contains(model.Street));
            }
            if (model.HasBalance.HasValue && model.HasBalance == true)
            {
                citations = citations.Where(s => s.FineAmount.HasValue)
                    .Where(m => m.FineAmount.Value > 0)
                    .Where(m => m.Balance.HasValue)
                    .Where(m => m.Balance.Value == 0);
            }
            if (model.HasBalance.HasValue && model.HasBalance == false)
            {
                citations = citations.Where(s => !s.FineAmount.HasValue);
            }


            return citations;
        }
        #endregion

    }
}

