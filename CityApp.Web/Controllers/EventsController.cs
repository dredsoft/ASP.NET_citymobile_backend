using AutoMapper;
using CityApp.Common.Caching;
using CityApp.Common.Extensions;
using CityApp.Common.Models;
using CityApp.Data;
using CityApp.Data.Enums;
using CityApp.Data.Models;
using CityApp.Services;
using CityApp.Services.Models;
using CityApp.Web.Filters;
using CityApp.Web.Models;
using CityApp.Web.Models.AccountSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Controllers
{
    [RequiresPermission(AccountPermissions.AccountAdministrator)]
    public class EventsController : AccountBaseController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<AccountSettingsController>();

        private AccountContext _accountCtx;
        private CommonContext _commonContext;
        private readonly MailService _mailSvc;
        private readonly ViolationService _violationSvc;
        private readonly PushNotification _PushSvc;
        private readonly AppSettings _appSettings;
        private readonly FileService _fileService;
        private readonly CommonAccountService _commonAccountSvc;

        public EventsController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache redisCache, IOptions<AppSettings> appSettings, AccountContext accountCtx, CommonUserService commonUserSvc, MailService mailSvc, ViolationService violationSvc, FileService fileService, CommonAccountService commonAccountSvc, PushNotification pushSvc)
            : base(commonContext, serviceProvider, redisCache, appSettings)
        {
            _accountCtx = accountCtx;
            _mailSvc = mailSvc;
            _violationSvc = violationSvc;
            _appSettings = appSettings.Value;
            _commonContext = commonContext;
            _fileService = fileService;
            _commonAccountSvc = commonAccountSvc;
            _PushSvc = pushSvc;
        }


        #region Events


        public async Task<IActionResult> Index()
        {
            EventListViewModel model = new EventListViewModel();
            var eventModel = await _accountCtx.Events.ForAccount(CommonAccount.Id).ToListAsync();
            model.EventListItem = Mapper.Map<List<EventListItem>>(eventModel);

            return View(model);
        }

        public IActionResult Create()
        {
            if (!CommonAccount.Features.HasFlag(AccountFeatures.Info))
            {
                return RedirectToAction("Index", "Home", new { accountNum = CommonAccount.Number });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventViewModel model)
        {

            if (ModelState.IsValid)
            {
                Event events = new Event()
                {
                    AccountId = CommonAccount.Id,
                    Title = model.Title,
                    Body = model.Body,
                    CreateUserId = User.GetLoggedInUserId().Value,
                    UpdateUserId = User.GetLoggedInUserId().Value
                };


                if (model.Start.HasValue)
                {
                    events.Start = model.Start.Value.ToUniversalTime();
                }

                if (model.End.HasValue)
                {
                    events.End = model.End.Value.ToUniversalTime();
                }

                _accountCtx.Events.Add(events);
                await _accountCtx.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            return View();
        }


        public async Task<IActionResult> Edit(Guid Id)
        {
            EventViewModel model = new EventViewModel();
            var eventModel = await _accountCtx.Events.ForAccount(CommonAccount.Id)
                .Where(x => x.Id == Id).SingleOrDefaultAsync();

            var violations = await _accountCtx.Violations.ForAccount(CommonAccount.Id).Include(m => m.Category).ThenInclude(m => m.Type)
                .Where(m => m.Category.Name.Contains("Parking"))
                .ToListAsync();
            var commonAccount = await _commonContext.CommonAccounts.Include(m => m.City).Where(m => m.Id == CommonAccount.Id).SingleAsync();

            model = Mapper.Map<EventViewModel>(eventModel);
            model.Latitude = commonAccount.City.Latitude;
            model.Longitude = commonAccount.City.Longitude;

            if (model.Start.HasValue)
            {
                model.Start = model.Start.Value.UTCToAccountLocalTime(CommonAccount.CityTimeZone);
            }

            if (model.End.HasValue)
            {
                model.End = model.End.Value.UTCToAccountLocalTime(CommonAccount.CityTimeZone);
            }

            model.Violations = Mapper.Map<List<AccountViolationListItem>>(violations);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var eventModel = await _accountCtx.Events.ForAccount(CommonAccount.Id).Where(x => x.Id == model.Id).SingleOrDefaultAsync();
                if (eventModel != null)
                {
                    eventModel.Title = model.Title;
                    eventModel.Body = model.Body;
                    eventModel.UpdateUserId = User.GetLoggedInUserId().Value;
                    eventModel.Start = model.Start;
                    eventModel.End = model.End;

                    if (eventModel.Start.HasValue)
                    {
                        eventModel.Start = model.Start.Value.LocalToUTC(CommonAccount.CityTimeZone);
                    }
                    if (eventModel.End.HasValue)
                    {
                        eventModel.End = model.End.Value.LocalToUTC(CommonAccount.CityTimeZone);
                    }

                    _accountCtx.Events.Update(eventModel);
                    await _accountCtx.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

            }
            var violations = await _accountCtx.Violations.ForAccount(CommonAccount.Id).Include(m => m.Category).ThenInclude(m => m.Type).ToListAsync();
            var commonAccount = await _commonContext.CommonAccounts.Include(m => m.City).Where(m => m.Id == CommonAccount.Id).SingleAsync();

            model.Violations = Mapper.Map<List<AccountViolationListItem>>(violations);
            model.Latitude = commonAccount.City.Latitude;
            model.Longitude = commonAccount.City.Longitude;

            return View(model);
        }

        public async Task<IActionResult> Delete(Guid Id)
        {
            var result = new ServiceResponse<EventViewModel>();
            var eventModel = await _accountCtx.Events.ForAccount(CommonAccount.Id).Where(x => x.Id == Id).SingleOrDefaultAsync();
            var eventPricing = await _accountCtx.EventViolationPrices.ForAccount(CommonAccount.Id).Where(x => x.EventId == eventModel.Id).ToListAsync();
            var eventCoordinates = await _accountCtx.EventBoundaryCoordinates.ForAccount(CommonAccount.Id).Where(x => x.EventId == eventModel.Id).ToListAsync();
            if (eventModel != null)
            {
                _accountCtx.EventViolationPrices.RemoveRange(eventPricing);
                _accountCtx.EventBoundaryCoordinates.RemoveRange(eventCoordinates);
                _accountCtx.Events.Remove(eventModel);
                await _accountCtx.SaveChangesAsync();
                result.Success = true;
                result.Data = null;
            }

            return Json(result);

        }

        #endregion


        #region Coordinates

        public async Task<IActionResult> GetCoordinates(Guid id)
        {
            var model = new ServiceResponse<List<EventBoundaryCoordinate>>();

            var eventCoordinates = await _accountCtx.EventBoundaryCoordinates.ForAccount(CommonAccount.Id)
                .Where(m => m.EventId == id)
                .OrderBy(m => m.Order)
                .ToListAsync();
            model.Data = eventCoordinates;
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCoordinates([FromBody]List<EventCoordinatesViewModel> coordinates)
        {
            if(coordinates.Any())
            {
                var firstEvent = coordinates.First();
                await DeleteEventCoordinates(firstEvent.EventId);

                var eventboundaries = Mapper.Map<List<EventBoundaryCoordinate>>(coordinates);

                eventboundaries.ForEach(a => {
                    a.AccountId = CommonAccount.Id;
                    a.CreateUserId = LoggedInUser.Id;
                    a.UpdateUserId = LoggedInUser.Id;
                    a.CreateUtc = DateTime.UtcNow;
                    a.UpdateUtc = DateTime.UtcNow;
                });

                 _accountCtx.EventBoundaryCoordinates.AddRange(eventboundaries);

                await _accountCtx.SaveChangesAsync();

                var eventBoundaryCoordinates = Mapper.Map<List<EventBoundaryCoordinate>>(coordinates);
            }

            return Ok(true);
        }

        public async Task<IActionResult> DeleteCoordinates(Guid id)
        {
            await DeleteEventCoordinates(id);
            return Ok();
        }

        private async Task DeleteEventCoordinates(Guid eventId)
        {
            var eventCoordinates = await _accountCtx.EventBoundaryCoordinates.ForAccount(CommonAccount.Id).Where(m => m.EventId == eventId).ToListAsync();
            _accountCtx.EventBoundaryCoordinates.RemoveRange(eventCoordinates);

            await _accountCtx.SaveChangesAsync();
        }


        #endregion

        #region Pricing

        public async Task<IActionResult> GetPricing(Guid id)
        {
            var model = new ServiceResponse<List<EventViolationPrice>>();

            var eventCoordinates = await _accountCtx.EventViolationPrices.Include(m => m.Violation).ForAccount(CommonAccount.Id)
                .Where(m => m.EventId == id)
                .ToListAsync();
            model.Data = eventCoordinates;
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddPricing([FromBody]EventPricingViewModel model)
        {
            var response = new ServiceResponse<EventPricingViewModel>();
            if (ModelState.IsValid)
            {
                var price = Mapper.Map<EventViolationPrice>(model);
                price.AccountId = CommonAccount.Id;
                price.CreateUserId = LoggedInUser.Id;
                price.UpdateUserId = LoggedInUser.Id;
                price.CreateUtc = DateTime.UtcNow;
                price.CreateUtc = DateTime.UtcNow;

                _accountCtx.EventViolationPrices.Add(price);

                await _accountCtx.SaveChangesAsync();

                response.Data = Mapper.Map<EventPricingViewModel>(price);
            }
            else
            {
                
            }

            return Ok(response);
        }

        public async Task<IActionResult> DeletePricing(Guid id)
        {
            var response = new ServiceResponse<bool>();

            var eventViolation = await _accountCtx.EventViolationPrices.ForAccount(CommonAccount.Id).Where(m => m.Id == id).SingleOrDefaultAsync();

            if(eventViolation != null)
            {
                _accountCtx.EventViolationPrices.Remove(eventViolation);
                await _accountCtx.SaveChangesAsync();
            }

            return Ok(response);
        }


        #endregion

    }
}
