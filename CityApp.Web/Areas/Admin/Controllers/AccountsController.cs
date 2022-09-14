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
using CityApp.Areas.Admin.Models;
using CityApp.Services;
using CityApp.Data.Enums;
using CityApp.Common.Extensions;
using CityApp.Services.Models;
using Microsoft.AspNetCore.Hosting;
using CityApp.Common.Utilities;
using CityApp.Data.Extensions;

namespace CityApp.Web.Areas.Admin.Controllers
{

    public class AccountsController : BaseAdminController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<AccountsController>();
        private readonly CommonAccountService _commonAccountSvc;
        private readonly IHostingEnvironment _env;

        public AccountsController(CommonContext commonContext, RedisCache redisCache, IServiceProvider serviceProvider, IOptions<AppSettings> appSettings, CommonAccountService commonAccountSvc, IHostingEnvironment env)
            : base(commonContext, redisCache, serviceProvider, appSettings)
        {
            _commonAccountSvc = commonAccountSvc;
            _env = env;
        }

        #region Account Listing
        /// <summary>
        /// get listing of all accounts
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Index(AccountListViewModel model)
        {
            var loggedInUser = LoggedInUser;

            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            var accounts = CommonContext.CommonAccounts
              .Include(m => m.City)
              .Include(m => m.OwnerUser)
              .Include(m => m.Partition).AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Searchstring))
            {
                accounts = accounts.Where(s =>
                            s.Name.ToLower().Contains(model.Searchstring.ToLower())
                            || s.City.Name.ToLower().Contains(model.Searchstring.ToLower()));
            }

            switch (model.SortOrder)
            {
                case "CityName":
                    if (model.SortDirection == "DESC")
                    {
                        accounts = accounts.OrderByDescending(s => s.City.Name);
                    }
                    else
                    {
                        accounts = accounts.OrderBy(s => s.City.Name);
                    }
                    break;
                case "Partition":
                    if (model.SortDirection == "DESC")
                    {
                        accounts = accounts.OrderByDescending(s => s.Partition.Name);
                    }
                    else
                    {
                        accounts = accounts.OrderBy(s => s.Partition.Name);
                    }
                    break;
                case "FullName":
                    if (model.SortDirection == "DESC")
                    {
                        accounts = accounts.OrderByDescending(s => s.OwnerUser.FirstName);
                    }
                    else
                    {
                        accounts = accounts.OrderBy(s => s.OwnerUser.FirstName);
                    }
                    break;
                default:
                    if (model.SortDirection == "DESC")
                    {
                        accounts = accounts.OrderByDescending(s => s.Name);
                    }
                    else
                    {
                        accounts = accounts.OrderBy(s => s.Name);
                    }
                    break;
            }

            var totalQueryCount = await accounts.CountAsync();

            model.Accounts = Mapper.Map<List<AccountListItem>>(await accounts.Skip(offset).Take(model.PageSize).ToListAsync());

            model.Paging.CurrentPage = currentPageNum;
            model.Paging.ItemsPerPage = model.PageSize;
            model.Paging.TotalItems = totalQueryCount;

            return View(model);
        }

        /// <summary>
        /// go back to account listing page
        /// </summary>
        /// <returns></returns>
        public IActionResult Cancel()
        {
            return RedirectToAction("Index", "Accounts", new { Area = Area.Admin });
        }
        #endregion

        #region Create Account
        /// <summary>
        /// create a new account
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Create()
        {
            var model = new AccountViewModel();

            await PopulateDropDownAccount(model);
            return View(model);
        }

        /// <summary>
        /// click on save button for creating a new account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Create(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = new CommonAccount()
                {
                    Name = model.Name.Trim(),
                    CityId = model.CityId,
                    PartitionId = model.PartitionId,
                    Partition = await CommonContext.Partitions.SingleAsync(m => m.Id == model.PartitionId),
                    StorageBucketName = model.StorageBucketName,
                    CitationWorkflow = model.CitationWorkflow,
                    Features = model.Features
                };
                if (Convert.ToString(model.OwnerId) != "00000000-0000-0000-0000-000000000000")
                {
                    account.OwnerUserId = model.OwnerId;
                }

                var NameAlreadyExists = await CommonContext.CommonAccounts.AnyAsync(q => q.Name.ToLower() == model.Name.Trim().ToLower());

                if (NameAlreadyExists)
                {
                    // This isn't a security risk because we've verified the Name already exists
                    ModelState.AddModelError(string.Empty, "Name already exists.");
                }
                else
                {
                    var result = await _commonAccountSvc.CreateAccount(account, User.GetLoggedInUserId().Value, model.AccViolationType, model.CitationCounter);

                    //Purge common accounts cache
                    await _cache.RemoveAsync(WebCacheKey.CommonAccounts);

                    return RedirectToAction("Index");
                }

            }

            await PopulateDropDownAccount(model);


            return View(model);
        }
        #endregion

        #region Edit Account

        /// <summary>
        /// get data for edit an existing account
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <returns></returns>
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Edit(int AccountNumber)
        {
            var model = new AccountViewModel();
            model.AccountNumber = AccountNumber;
            await PopulateAllFields(model);
            return View(model);
        }

        /// <summary>
        /// click on save button for update an existing account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Edit(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Convert.ToString(model.OwnerId) != "00000000-0000-0000-0000-000000000000")
                {
                    var account = new CommonAccount()
                    {
                        Name = model.Name.Trim(),
                        Id = model.Id,
                        PartitionId = model.PartitionId,
                        OwnerUserId = model.OwnerId,
                        StorageBucketName = model.StorageBucketName,
                        CitationWorkflow = model.CitationWorkflow,
                        Features = model.Features
                    };

                    var NameAlreadyExists = await CommonContext.CommonAccounts.AnyAsync(q => q.Name.ToLower() == model.Name.Trim().ToLower() && q.Id != model.Id);

                    if (NameAlreadyExists)
                    {
                        // This isn't a security risk because we've verified the Name already exists
                        ModelState.AddModelError(string.Empty, "Name already exists.");
                    }
                    else
                    {
                        var result = await _commonAccountSvc.UpdateAccount(account, User.GetLoggedInUserId().Value, model.AccountNumber, model.AccViolationType);

                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "You must be select the owner.");
                }
            }

            await PopulateAllFields(model);

            return View(model);
        }
        #endregion

        #region Account Violation
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Violations(Guid AccountId, AccViolationListViewModel model)
        {
            // var model = new ViolationListViewModel();

            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            //get commom violation type
            var vioTypeId = await CommonContext.CommonAccountViolationTypes.AsNoTracking()
                                .Where(s => s.AccountId.Equals(model.AccountId)).Select(m => m.ViolationTypeId).ToListAsync();

            var accountDetail = await CommonContext.CommonAccounts.SingleOrDefaultAsync(account => account.Id == model.AccountId);
            var PartitionDetail = await CommonContext.Partitions.SingleOrDefaultAsync(partition => partition.Id == accountDetail.PartitionId);
            var CityDetail = await CommonContext.Cities.SingleOrDefaultAsync(city => city.Id == accountDetail.CityId);

            var _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));

            //get Violation that are disabled or that is related to passed accountid
            var violations = _accountCtx.Violations
                .Include(m => m.Category)
                .Include(m => m.Category.Type)
                .ForAccount(model.AccountId)
                .AsQueryable();

            //Filter the violation by the types that this account is associated with.
            violations = violations.Where(m => vioTypeId.Contains(m.Category.Type.CommonViolationTypeId));

            if (Convert.ToString(model.CategoryId) != "00000000-0000-0000-0000-000000000000")
            {
                violations = violations.Where(s => s.CategoryId.Equals(model.CategoryId));
            }
            if (Convert.ToString(model.TypeId) != "00000000-0000-0000-0000-000000000000")
            {
                violations = violations.Where(s => s.Category.TypeId.Equals(model.TypeId));
            }

            //sorting
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
                case "Disabled":
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.Disabled);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.Disabled);
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

            model.Violation = Mapper.Map<List<AccViolationListItem>>(await violations.Skip(offset).Take(model.PageSize).ToListAsync());

            model.Paging.CurrentPage = currentPageNum;
            model.Paging.ItemsPerPage = model.PageSize;
            model.Paging.TotalItems = totalQueryCount;
            model.AccountId = model.AccountId;
            model.CityName = CityDetail.Name;

            //TODO: this should be cached and retrieved from a service
            var Types = await _accountCtx.ViolationTypes.Where(m => m.AccountId.Equals(model.AccountId) && vioTypeId.Contains(m.CommonViolationTypeId)).Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString(),
            })
            .ToListAsync();

            model.Types.AddRange(Types);

            //TODO: this should be cached and retrieved from a service
            var Categories = await _accountCtx.ViolationCategorys.Include(m => m.Type).Where(m => m.AccountId.Equals(model.AccountId) && vioTypeId.Contains(m.Type.CommonViolationTypeId)).Select(m => new SelectListItem
            {
                Text = m.Name + " (" + m.Type.Name + " )",
                Value = m.Id.ToString(),

            }).ToListAsync();

            model.Categories.AddRange(Categories);

            return View(model);
        }


        public async Task<IActionResult> CreateViolation(Guid accountId)
        {
            var vm = new AdminAccountViolationViewModel { AccountId = accountId };

            var acctContext = await GetAccountContext(accountId);
            vm.Categories =await GetViolationCategories(acctContext, accountId);

            return View(vm);
        }

        [Authorize(Roles = SystemPermission.Administrator)]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateViolation(AdminAccountViolationViewModel model)
        {
            var acctCtx = await GetAccountContext(model.AccountId);

            if (ModelState.IsValid)
            {
                //Transform ViewModel into Entity Model
                var violation = Mapper.Map<Violation>(model);
                violation.CreateUserId = LoggedInUser.Id;
                violation.UpdateUserId = LoggedInUser.Id;

                acctCtx.Violations.Add(violation);

                try
                {
                   await acctCtx.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("",ex.Message);
                }

                //Save and redirect to Violation List

                return RedirectToAction("Violations", new { accountId = model.AccountId });
            }

            model.Categories = await GetViolationCategories(acctCtx, model.AccountId);

            return View(model);
         

        }

        public async Task<IActionResult> EditViolation(Guid id, Guid accountId)
        {
            var acctContext = await GetAccountContext(accountId);

            var violation = await acctContext.Violations.Where(m => m.AccountId == accountId && m.Id == id).SingleAsync();

            var vm = Mapper.Map<AdminAccountViolationViewModel>(violation);

            vm.Categories = await GetViolationCategories(acctContext, accountId);

            return View(vm);
        }

        [Authorize(Roles = SystemPermission.Administrator)]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditViolation(AdminAccountViolationViewModel model)
        {
            var acctContext = await GetAccountContext(model.AccountId);

            if (ModelState.IsValid)
            {
                var violation = await acctContext.Violations.Where(m => m.AccountId == model.AccountId && m.Id == model.Id).SingleAsync();
                //Transform ViewModel into Entity Model
                Mapper.Map(model, violation);
                violation.UpdateUserId = LoggedInUser.Id;
                violation.UpdateUtc = DateTime.UtcNow;

                try
                {
                    await acctContext.SaveChangesAsync();
                    return RedirectToAction("Violations", new { accountId = model.AccountId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }

            }

            model.Categories = await GetViolationCategories(acctContext, model.AccountId);

            return View(model);
            
        }

        #endregion

        #region Account Categories
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Categories(Guid AccountId)
        {
            var account = await CommonContext.CommonAccounts.SingleAsync(m => m.Id == AccountId);

            var acctCtx = await GetAccountContext(AccountId);
            var categories = await acctCtx.ViolationCategorys.Include(m => m.Type).Where(m => m.AccountId == AccountId).ToListAsync();

            var vm = new AccountCategoryIndexViewModel { Account = account, AccountId = account.Id, Categories= categories };

            return View(vm);
        }


        public async Task<IActionResult> CreateCategory(Guid accountId)
        {
            var vm = new AdminAccountCategoryViewModel { AccountId = accountId };

            var acctContext = await GetAccountContext(accountId);
            vm.Types = await GetViolationTypes(acctContext, accountId);

            return View(vm);
        }

        [Authorize(Roles = SystemPermission.Administrator)]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateCategory(AdminAccountCategoryViewModel model)
        {
            var acctCtx = await GetAccountContext(model.AccountId);

            if (ModelState.IsValid)
            {
                //Transform ViewModel into Entity Model
                var category = Mapper.Map<ViolationCategory>(model);
                category.CreateUserId = LoggedInUser.Id;
                category.UpdateUserId = LoggedInUser.Id;

                acctCtx.ViolationCategorys.Add(category);

                try
                {
                    await acctCtx.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }

                //Save and redirect to Violation List

                return RedirectToAction("Categories", new { accountId = model.AccountId });
            }

            model.Types = await GetViolationTypes(acctCtx, model.AccountId);

            return View(model);


        }

        public async Task<IActionResult> EditCategory(Guid id, Guid accountId)
        {
            var acctContext = await GetAccountContext(accountId);

            var category = await acctContext.ViolationCategorys.Where(m => m.AccountId == accountId && m.Id == id).SingleAsync();

            var vm = Mapper.Map<AdminAccountCategoryViewModel>(category);

            vm.Types = await GetViolationTypes(acctContext, accountId);

            return View(vm);
        }

        [Authorize(Roles = SystemPermission.Administrator)]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditCategory(AdminAccountCategoryViewModel model)
        {
            var acctContext = await GetAccountContext(model.AccountId);

            if (ModelState.IsValid)
            {
                var violation = await acctContext.ViolationCategorys.Where(m => m.AccountId == model.AccountId && m.Id == model.Id).SingleAsync();
                //Transform ViewModel into Entity Model
                Mapper.Map(model, violation);
                violation.UpdateUserId = LoggedInUser.Id;
                violation.UpdateUtc = DateTime.UtcNow;

                try
                {
                    await acctContext.SaveChangesAsync();
                    return RedirectToAction("Categories", new { accountId = model.AccountId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }

            }

            model.Types = await GetViolationTypes(acctContext, model.AccountId);

            return View(model);

        }

        #endregion


        #region
        /// <summary>
        /// bind the City, Partition, owner and Violation Type dropdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task PopulateDropDownAccount(AccountViewModel model)
        {
            //TODO: this should be cached and retrieved from a service
            var citites = await CommonContext.Cities.Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() }).ToListAsync();
            model.Cities.AddRange(citites);

            //TODO: this should be cached and retrieved from a service
            var partitiions = await CommonContext.Partitions
                .Where(m => m.Disabled == false)
                .Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() }).ToListAsync();
            model.Partitions.AddRange(partitiions);

            //TODO: this should be cached and retrieved from a service
            var owners = await CommonContext.Users.Select(m => new SelectListItem { Text = m.Email, Value = m.Id.ToString() }).ToListAsync();
            model.Users.AddRange(owners);

            model.AccViolationType.Clear();
            //TODO: this should be cached and retrieved from a service
            var Types = await CommonContext.CommonViolationTypes
               .Select(m => new AccountViolationType { TypeName = m.Name, TypeId = m.Id }).ToListAsync();
            model.AccViolationType.AddRange(Types);

            //Add list of buckets for amazon account
            model.Buckets = GetBuckets();

            var enulist = Enum.GetValues(typeof(CitationWorkflow)).Cast<CitationWorkflow>().Select(v => new SelectListItem
            {
                Text = v.GetEnumName(),
                Value = ((int)v).ToString()
            }).ToList();

            model.CitationWorkflowItems.AddRange(enulist);
            model.CitationWorkflow = CitationWorkflow.None;

        }

        private List<SelectListItem> GetBuckets()
        {
            var result = new List<SelectListItem>();

            var environment = "dev";
            if (_env.EnvironmentName == EnvironmentName.Staging)
            {
                environment = "staging";
            }
            if (_env.EnvironmentName == EnvironmentName.Production)
            {
                environment = "prod";
            }

            result.Add(new SelectListItem { Text = $"cityapp.{environment}.west", Value = $"cityapp.{environment}.west" });
            result.Add(new SelectListItem { Text = $"cityapp.{environment}.east", Value = $"cityapp.{environment}.east" });

            return result;
        }
        #endregion

        #region
        /// <summary>
        /// fill all fields for edit record using Id
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task PopulateAllFields(AccountViewModel model)
        {
            var accountDetail = await CommonContext.CommonAccounts
                .Include(m => m.City)
                .Include(m => m.Partition)
                .SingleOrDefaultAsync(account => account.Number == model.AccountNumber);

            var cityDetail = accountDetail.City;
            var partitionDetails = accountDetail.Partition;
            var accountViolations = await CommonContext.CommonAccountViolationTypes.Where(av => av.AccountId == accountDetail.Id).ToListAsync();

            model.Name = accountDetail.Name;
            model.CityName = cityDetail.Name;
            model.PartitionName = partitionDetails.Name;
            model.PartitionId = accountDetail.PartitionId;
            model.CityId = accountDetail.CityId;
            model.Id = accountDetail.Id;
            model.OwnerId = accountDetail.OwnerUserId.Value;
            model.StorageBucketName = accountDetail.StorageBucketName;
            model.CitationWorkflow = accountDetail.CitationWorkflow;
            model.Features = accountDetail.Features;

            //TODO: this should be cached and retrieved from a service
            var owners = await CommonContext.Users.Select(m => new SelectListItem
            {
                Text = m.Email,
                Value = m.Id.ToString(),
                Selected = (m.Id == accountDetail.OwnerUserId ? true : false)
            }).ToListAsync();

            model.Users.AddRange(owners);

            model.AccViolationType.Clear();

            //TODO: this should be cached and retrieved from a service
            var Types = await CommonContext.CommonViolationTypes
               .Select(m => new AccountViolationType { TypeName = m.Name, TypeId = m.Id }).ToListAsync();
            model.AccViolationType.AddRange(Types);

            foreach (var Type in model.AccViolationType)
            {
                foreach (var selected in accountViolations)
                {
                    if (Type.TypeId == selected.ViolationTypeId)
                    {
                        Type.IsCheckedViolation = true;
                    }
                }
            }

            var enulist = Enum.GetValues(typeof(CitationWorkflow)).Cast<CitationWorkflow>().Select(v => new SelectListItem
            {
                Text = v.GetEnumName(),
                Value = ((int)v).ToString()
            }).ToList();

            model.CitationWorkflowItems.AddRange(enulist);
            model.Buckets = GetBuckets();
        }
        #endregion

        #region City
        /// <summary>
        /// get cities based on searchTerm
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCities(string searchTerm, int pageSize, int pageNum)
        {
            Select2PagedResult PagedCity = new Select2PagedResult();
            try
            {
                pageNum = pageNum == 0 ? 1 : pageNum;
                var offset = (pageSize * pageNum) - pageSize;
                var accounts = CommonContext.Cities.AsQueryable();

                accounts = accounts
                    .Where(s => s.Name.ToLower().Contains(searchTerm.ToLower()))
                    .OrderBy(s => s.StateCode).ThenBy(n => n.Name);

                List<City> citiess = await accounts.Skip(offset).Take(pageSize).ToListAsync();

                var totalQueryCount = await accounts.CountAsync();

                PagedCity = CitiesToSelect2Format(citiess, totalQueryCount);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(PagedCity);

        }

        /// <summary>
        /// get cities
        /// </summary>
        /// <param name="ctiess"></param>
        /// <param name="totalQueryCount"></param>
        /// <returns></returns>
        private Select2PagedResult CitiesToSelect2Format(List<City> ctiess, int totalQueryCount)
        {
            Select2PagedResult jsonCities = new Select2PagedResult();
            var citites = ctiess.Select(a => new SelectListItem { Text = a.Name + " (" + a.StateCode + " )", Value = a.Id.ToString() });
            jsonCities.Results.AddRange(citites);

            //Set the total count of the results from the query.
            jsonCities.Total = totalQueryCount;

            return jsonCities;
        }
        #endregion


        public async Task<IActionResult> PurgeCache()
        {
            var accountUsers = await CommonContext.UserAccounts.Include(m => m.Account).AsNoTracking().ToListAsync();
            var cachKeys = new List<string>();

            var accounts = accountUsers.Select(m => m.Account).Distinct();

            //CommonAccounts
            foreach (var account in accounts)
            {
                cachKeys.Add(WebCacheKey.CommonAccount(account.Number));
            }
            cachKeys.Add(WebCacheKey.CommonAccounts);
            
            //Account Users
            foreach(var accountUser in accountUsers)
            {
                cachKeys.Add(WebCacheKey.CommonUserAccount(accountUser.Account.Number, accountUser.UserId.Value));
            }


            var users = await CommonContext.Users.Select(m => m.Id).ToListAsync();
            foreach(var user in users)
            {
                cachKeys.Add(WebCacheKey.LoggedInUser(user));
            }

            cachKeys.Add(WebCacheKey.HealthCheck);
            cachKeys.Add(WebCacheKey.CommonViolationTypes);
            cachKeys.Add(WebCacheKey.CommonViolationCategories);
            cachKeys.Add(WebCacheKey.CommonViolations);
            cachKeys.Add(WebCacheKey.Violations);

            await _cache.RemoveAsync(cachKeys.ToArray());

            return RedirectToAction("Index");

        }


        /// <summary>
        /// get cities based on searchTerm
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateDisableStatus(bool Disabled, Guid ViolationId, Guid AccountId)
        {
            var result = new ServiceResponse<Violation>();
            try
            {
                var accountDetail = await CommonContext.CommonAccounts.SingleOrDefaultAsync(account => account.Id == AccountId);
                var PartitionDetail = await CommonContext.Partitions.SingleOrDefaultAsync(partition => partition.Id == accountDetail.PartitionId);

                var _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));

                result = await _commonAccountSvc.UpdateDisableStatus(Disabled, ViolationId, _accountCtx);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result);

        }

        private async Task<AccountContext> GetAccountContext(Guid accountId)
        {
            var accountDetail = await CommonContext.CommonAccounts.Include(m => m.Partition).SingleOrDefaultAsync(account => account.Id == accountId);

            var _accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));

            return _accountCtx;
        }

        private async Task<SelectList> GetViolationCategories(AccountContext acctCtx, Guid id)
        {
            var violationCategories = await acctCtx.ViolationCategorys.Include(m => m.Type).ForAccount(id).ToListAsync();

            violationCategories.ForEach(m => {
                m.Name = m.Name + $" ({m.Type.Name})";
            });

            return new SelectList(violationCategories, "Id", "Name");
        }

        private async Task<SelectList> GetViolationTypes(AccountContext acctCtx, Guid id)
        {
            var violationCategories = await acctCtx.ViolationTypes.ForAccount(id).ToListAsync();

            return new SelectList(violationCategories, "Id", "Name");
        }

    }
}
