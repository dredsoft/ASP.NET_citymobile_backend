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
using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using CityApp.Common.Extensions;

namespace CityApp.Web.Areas.Admin.Controllers
{
    public class ViolationsController : BaseAdminController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<ViolationsController>();
        private readonly CommonAccountService _commonAccountSvc;
        private readonly ViolationService _violationSvc;


        public ViolationsController(CommonContext commonContext, RedisCache redisCache, IServiceProvider serviceProvider, IOptions<AppSettings> appSettings, CommonAccountService commonAccountSvc, ViolationService violationSvc)
            : base(commonContext, redisCache, serviceProvider, appSettings)
        {
            _commonAccountSvc = commonAccountSvc;
            _violationSvc = violationSvc;
        }


        #region Type
        /// <summary>
        /// listing of violation Type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> Types(ViolationTypeListViewModel model)
        {
            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            var violations = CommonContext.CommonViolationTypes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Searchstring))
            {
                violations = violations.Where(s =>
                            s.Name.ToLower().Contains(model.Searchstring.ToLower()));
            }

            switch (model.SortOrder)
            {

                case "Description":
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.Description);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.Description);
                    }
                    break;
                default:
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.Name);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.Name);
                    }
                    break;
            }


            var totalQueryCount = await violations.CountAsync();

            model.Violations = Mapper.Map<List<ViolationTypeListItem>>(await violations.Skip(offset).Take(model.PageSize).ToListAsync());

            model.Paging.CurrentPage = currentPageNum;
            model.Paging.ItemsPerPage = model.PageSize;
            model.Paging.TotalItems = totalQueryCount;

            return View(model);
        }

        /// <summary>
        /// call create violation type view
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = SystemPermission.Administrator)]
        public IActionResult CreateType()
        {
            var model = new ViolationTypeViewModel();

            return View(model);
        }

        /// <summary>
        /// call on create violation type button
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> CreateType(ViolationTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var type = new CommonViolationType()
                {
                    Name = model.Name.Trim(),
                    Description = model.Description?.Trim(),
                    Disabled = model.Disabled
                };

                var NameAlreadyExists = await CommonContext.CommonViolationTypes.AnyAsync(q => q.Name.ToLower() == model.Name.Trim().ToLower());

                if (NameAlreadyExists)
                {
                    // This isn't a security risk because we've verified the Name already exists
                    ModelState.AddModelError(string.Empty, "Name already exists.");
                }
                else
                {
                    var result = await _violationSvc.CreateViolationType(type, User.GetLoggedInUserId().Value);

                    //Purge common accounts cache
                    await _cache.RemoveAsync(WebCacheKey.CommonViolationTypes);

                    return RedirectToAction("Types");
                }
            }

            return View(model);
        }

        /// <summary>
        /// call edit violation type view
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> EditType(Guid Id)
        {
            var model = new ViolationTypeViewModel();

            //get data from database by id
            var violationTypeDetails = await CommonContext.CommonViolationTypes.SingleOrDefaultAsync(violation => violation.Id == Id);
            model.Name = violationTypeDetails.Name;
            model.Description = violationTypeDetails.Description;
            model.Disabled = violationTypeDetails.Disabled;
            model.Id = violationTypeDetails.Id;

            return View(model);
        }

        /// <summary>
        /// edit violation type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> EditType(ViolationTypeViewModel model)
        {

            if (ModelState.IsValid)
            {

                var violation = new CommonViolationType()
                {
                    Name = model.Name.Trim(),
                    Id = model.Id,
                    Description = model.Description?.Trim(),
                    Disabled = model.Disabled
                };
                var NameAlreadyExists = await CommonContext.CommonViolationTypes.AnyAsync(q => q.Name.ToLower() == model.Name.Trim().ToLower() && q.Id != model.Id);

                if (NameAlreadyExists)
                {
                    // This isn't a security risk because we've verified the Name already exists
                    ModelState.AddModelError(string.Empty, "Name already exists.");
                }
                else
                {
                    var result = await _violationSvc.UpdateType(violation, User.GetLoggedInUserId().Value);

                    //Purge common accounts cache
                    await _cache.RemoveAsync(WebCacheKey.CommonViolationTypes);

                    return RedirectToAction("Types");
                }
            }

            return View(model);
        }

        /// <summary>
        /// return back to violation type listing page
        /// </summary>
        /// <returns></returns>
        public IActionResult CancelType()
        {
            return RedirectToAction("Types", "Violations", new { Area = Area.Admin });
        }
        #endregion


        #region Category

        /// <summary>
        /// view category listing
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Categories(ViolationCategoryListViewModel model)
        {
            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            var violations = CommonContext.CommonViolationCategories
                .Include(m => m.Type).
                AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Searchstring))
            {
                if (Convert.ToString(model.TypeId) != "00000000-0000-0000-0000-000000000000")
                {
                    violations = violations.Where(s =>
                            s.Name.ToLower().Contains(model.Searchstring.ToLower()));

                    violations = violations.Where(s => s.TypeId.Equals(model.TypeId));
                }
                else
                {
                    violations = violations.Where(s =>
                           s.Name.ToLower().Contains(model.Searchstring.ToLower()));
                }
            }
            else if (Convert.ToString(model.TypeId) != "00000000-0000-0000-0000-000000000000")
            {
                violations = violations.Where(s => s.TypeId.Equals(model.TypeId));
            }


            switch (model.SortOrder)
            {

                case "Type":
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.Type.Name);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.Type.Name);
                    }
                    break;
                default:
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.Name);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.Name);
                    }
                    break;
            }

            var totalQueryCount = await violations.CountAsync();

            model.Violations = Mapper.Map<List<ViolationCategoryListItem>>(await violations.Skip(offset).Take(model.PageSize).ToListAsync());

            model.Paging.CurrentPage = currentPageNum;
            model.Paging.ItemsPerPage = model.PageSize;
            model.Paging.TotalItems = totalQueryCount;

            var Types = await CommonContext.CommonViolationTypes.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString()
            }).ToListAsync();

            model.Types.AddRange(Types);

            return View(model);

        }

        /// <summary>
        /// call create violation category view
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> CreateCategory()
        {
            var model = new ViolationTypeViewModel();
            await PopulateDropDownType(model);
            return View(model);
        }

        /// <summary>
        /// call on create violation category button
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> CreateCategory(ViolationTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Category = new CommonViolationCategory()
                {
                    Name = model.Name.Trim(),
                    Description = model.Description?.Trim(),
                    Disabled = model.Disabled,
                    TypeId = model.TypeId,
                    Type = await CommonContext.CommonViolationTypes.SingleAsync(m => m.Id == model.TypeId)
                };

                var NameAlreadyExists = await CommonContext.CommonViolationCategories.AnyAsync(
                    q => q.Name.ToLower() == model.Name.Trim().ToLower()
                    && q.TypeId == model.TypeId);

                if (NameAlreadyExists)
                {
                    // This isn't a security risk because we've verified the Name already exists
                    ModelState.AddModelError(string.Empty, "Violation Category Name with this Violation Type already exists.");
                }
                else
                {
                    var result = await _violationSvc.CreateViolationCategory(Category, User.GetLoggedInUserId().Value);

                    //Purge common Violation Categories cache
                    await _cache.RemoveAsync(WebCacheKey.CommonViolationCategories);
                    return RedirectToAction("Categories");
                }
            }

            await PopulateDropDownType(model);
            return View(model);
        }

        /// <summary>
        /// call edit violation category view
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> EditCategory(Guid Id)
        {
            var model = new ViolationTypeViewModel();

            //get data from database by id
            var violationTypeDetails = await CommonContext.CommonViolationCategories.SingleOrDefaultAsync(violation => violation.Id == Id);
            model.Name = violationTypeDetails.Name;
            model.Description = violationTypeDetails.Description;
            model.Disabled = violationTypeDetails.Disabled;
            model.Id = violationTypeDetails.Id;
            model.TypeId = violationTypeDetails.TypeId;

            await PopulateDropDownType(model);

            return View(model);
        }

        /// <summary>
        /// edit violation category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> EditCategory(ViolationTypeViewModel model)
        {
            if (ModelState.IsValid)
            {

                var violation = new CommonViolationCategory()
                {
                    Name = model.Name.Trim(),
                    Description = model.Description?.Trim(),
                    Disabled = model.Disabled,
                    TypeId = model.TypeId,
                    Type = await CommonContext.CommonViolationTypes.SingleAsync(m => m.Id == model.TypeId),
                    Id = model.Id
                };
                var NameAlreadyExists = await CommonContext.CommonViolationCategories.AnyAsync(
                    q => q.Name.ToLower() == model.Name.Trim().ToLower()
                    && q.TypeId == model.TypeId
                    && q.Id != model.Id);

                if (NameAlreadyExists)
                {
                    // This isn't a security risk because we've verified the Name already exists
                    ModelState.AddModelError(string.Empty, "Violation Category Name with this Violation Type already exists.");
                }
                else
                {
                    var result = await _violationSvc.UpdateViolationCategory(violation, User.GetLoggedInUserId().Value);
                    //Purge common Violation Category cache
                    await _cache.RemoveAsync(WebCacheKey.CommonViolationCategories);

                    return RedirectToAction("Categories");
                }
            }

            await PopulateDropDownType(model);
            return View(model);
        }

        /// <summary>
        /// return back to violation category page
        /// </summary>
        /// <returns></returns>
        public IActionResult CancelCategory()
        {
            return RedirectToAction("Categories", "Violations", new { Area = Area.Admin });
        }

        #endregion


        #region violation
        /// <summary>
        /// for violation
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index(ViolationListViewModel model)
        {
            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            var violations = CommonContext.CommonViolations
                .Include(m => m.Category)
                .Include(m => m.Category.Type)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Searchstring))
            {
                violations = violations.Where(s => s.Name.ToLower().Contains(model.Searchstring.ToLower()));
            }
            if (Convert.ToString(model.CategoryId) != "00000000-0000-0000-0000-000000000000")
            {
                violations = violations.Where(s => s.CategoryId.Equals(model.CategoryId));
            }
            if (Convert.ToString(model.TypeId) != "00000000-0000-0000-0000-000000000000")
            {
                violations = violations.Where(s => s.Category.TypeId.Equals(model.TypeId));
            }
            //if (model.Actions != false)
            //{
            ///  violations = violations.Where(s => s.Actions.Equals(model.Actions));
            //}

            switch (model.SortOrder)
            {
                case "Type":
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.Category.Type.Name);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.Category.Type.Name);
                    }
                    break;
                case "Category":
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.Category.Name);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.Category.Name);
                    }
                    break;
                case "HelpUrl":
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.HelpUrl);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.HelpUrl);
                    }
                    break;
                case "Towable":
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.Actions);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.Actions);
                    }
                    break;
                default:
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.Name);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.Name);
                    }
                    break;
            }

            var totalQueryCount = await violations.CountAsync();

            model.Violation = Mapper.Map<List<ViolationListItem>>(await violations.Skip(offset).Take(model.PageSize).ToListAsync());

            model.Paging.CurrentPage = currentPageNum;
            model.Paging.ItemsPerPage = model.PageSize;
            model.Paging.TotalItems = totalQueryCount;

            //TODO: this should be cached and retrieved from a service
            var Types = await CommonContext.CommonViolationTypes.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString(),
            }).ToListAsync();

            model.Types.AddRange(Types);

            var Categories = await CommonContext.CommonViolationCategories.Select(m => new SelectListItem
            {
                Text = m.Name + " (" + m.Type.Name + " )",
                Value = m.Id.ToString(),
            }).ToListAsync();

            model.Categories.AddRange(Categories);

            return View(model);
        }

        /// <summary>
        /// call create violation view
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Create()
        {
            var model = new ViolationViewModel();
            await PopulateDropDownCategory(model);
            return View(model);
        }

        /// <summary>
        /// call on create violation button
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Create(ViolationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Violation = new CommonViolation()
                {
                    Name = model.Name.Trim(),
                    Description = model.Description?.Trim(),
                    Disabled = model.Disabled,
                    HelpUrl = model.HelpUrl,
                    Actions = model.Actions,
                    CategoryId = model.CategoryId,
                    RequiredFields = model.Fields,
                    ReminderMessage = model.ReminderMessage,
                    ReminderMinutes = model.ReminderMinutes

                };

                var NameAlreadyExists = await CommonContext.CommonViolations.AnyAsync(
                   q => q.Name.ToLower() == model.Name.Trim().ToLower()
                   && q.CategoryId == model.CategoryId);

                if (NameAlreadyExists)
                {
                    // This isn't a security risk because we've verified the Name already exists
                    ModelState.AddModelError(string.Empty, "Violation Name with this Violation Category already exists.");
                }
                else
                {
                    var result = await _violationSvc.CreateViolation(Violation, User.GetLoggedInUserId().Value);
                    //Purge common Common Violations cache
                    await _cache.RemoveAsync(WebCacheKey.CommonViolations);
                    return RedirectToAction("Index");
                }
            }

            await PopulateDropDownCategory(model);
            return View(model);
        }

        /// <summary>
        /// call create violation view
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = new ViolationViewModel();

            //get data from database by id
            var violationDetails = await CommonContext.CommonViolations.SingleOrDefaultAsync(violation => violation.Id == Id);
            model.Name = violationDetails.Name;
            model.Description = violationDetails.Description;
            model.Disabled = violationDetails.Disabled;
            model.Id = violationDetails.Id;
            model.CategoryId = violationDetails.CategoryId;
            model.HelpUrl = violationDetails.HelpUrl;
            model.Actions = violationDetails.Actions;
            model.Fields = violationDetails.RequiredFields;
            model.ReminderMessage = violationDetails.ReminderMessage;
            model.ReminderMinutes = violationDetails.ReminderMinutes;


            await PopulateDropDownCategory(model);
            return View(model);
        }

        /// <summary>
        /// call on create violation button
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Edit(ViolationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Violation = new CommonViolation()
                {
                    Name = model.Name.Trim(),
                    Description = model.Description?.Trim(),
                    Disabled = model.Disabled,
                    HelpUrl = model.HelpUrl,
                    Actions = model.Actions,
                    CategoryId = model.CategoryId,
                    Category = await CommonContext.CommonViolationCategories.SingleAsync(m => m.Id == model.CategoryId),
                    Id = model.Id,
                    RequiredFields = model.Fields,
                    ReminderMinutes = model.ReminderMinutes,
                    ReminderMessage = model.ReminderMessage
                };

                var NameAlreadyExists = await CommonContext.CommonViolations.AnyAsync(
                 q => q.Name.ToLower() == model.Name.Trim().ToLower()
                 && q.CategoryId == model.CategoryId
                 && q.Id != model.Id
                 );

                if (NameAlreadyExists)
                {
                    // This isn't a security risk because we've verified the Name already exists
                    ModelState.AddModelError(string.Empty, "Violation Name with this Violation Category already exists.");
                }
                else
                {

                    var result = await _violationSvc.UpdateViolation(Violation, User.GetLoggedInUserId().Value);

                    //Purge common accounts cache
                    await _cache.RemoveAsync(WebCacheKey.CommonViolations);

                    return RedirectToAction("Index");

                }

            }
            await PopulateDropDownCategory(model);
            return View(model);
        }

        /// <summary>
        /// return back to violation page
        /// </summary>
        /// <returns></returns>
        public IActionResult Cancel()
        {
            return RedirectToAction("Index", "Violations", new { Area = Area.Admin });
        }

        #endregion

        #region
        private async Task PopulateDropDownType(ViolationTypeViewModel model)
        {
            //TODO: this should be cached and retrieved from a service
            var Types = await CommonContext.CommonViolationTypes.Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString(),
                Selected = (m.Id == model.TypeId ? true : false)
            }).ToListAsync();

            model.Types.AddRange(Types);

        }

        private async Task PopulateDropDownCategory(ViolationViewModel model)
        {
            var Categories = await CommonContext.CommonViolationCategories.Select(m => new SelectListItem
            {
                Text = m.Name + " (" + m.Type.Name + " )",
                Value = m.Id.ToString(),
            }).ToListAsync();

            model.Categories.AddRange(Categories);
        }
        #endregion

    }
}

