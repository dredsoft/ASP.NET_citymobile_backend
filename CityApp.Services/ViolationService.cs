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
using CityApp.Common.Utilities;
using CityApp.Data.Enums;

namespace CityApp.Services
{
    public class ViolationService : ICustomService
    {

        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        private static readonly object _randomLock = new object();
        private readonly CommonContext _commonCtx;
        private readonly AccountContext _accountCtx;
        private readonly RedisCache _cache;

        private static readonly ILogger _logger = Log.Logger.ForContext<ViolationService>();

        public ViolationService(CommonContext commonCtx, RedisCache cache, AccountContext accountCtx)
        {
            _commonCtx = commonCtx;
            _cache = cache;
            _accountCtx = accountCtx;
        }

        #region Type

        /// <summary>
        /// create new violation type
        /// </summary>
        /// <param name="violationtype"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<CommonViolationType>> CreateViolationType(CommonViolationType violationtype, Guid createdById)
        {
            var result = new ServiceResponse<CommonViolationType>();

            try
            {
                using (var scope = _commonCtx.Database.BeginTransaction())
                {
                    violationtype.CreateUserId = createdById;
                    violationtype.UpdateUserId = createdById;

                    _commonCtx.CommonViolationTypes.Add(violationtype);

                    await _commonCtx.SaveChangesAsync();

                    //Adds this newly created violation to all the account ViolationTypes.
                    await CreateAccountViolationType(violationtype);

                    scope.Commit();

                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating new Violation Type");

                throw ex;
            }

            return result;
        }


        /// <summary>
        /// Creates a new new violation type in the account context for every partition.
        /// </summary>
        public async Task CreateAccountViolationType(CommonViolationType commonViolationType)
        {
            var allAccounts = await _commonCtx.CommonAccounts.AsNoTracking().Include(m => m.Partition).Where(m => m.Archived == false).ToListAsync();

            //Go through each account and add this CommonViolation to Violation
            foreach (var account in allAccounts)
            {

                using (var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(account.Partition.ConnectionString)))
                {
                    var created = DateTime.UtcNow;
                    var violationType = Mapper.Map<ViolationType>(commonViolationType);

                    violationType.AccountId = account.Id;
                    violationType.CreateUtc = created; //Setting created and UpdatedUtc to the exact same value will let us know if this record has every been changed. 
                    violationType.UpdateUtc = created;

                    accountContext.ViolationTypes.Add(violationType);

                    await accountContext.SaveChangesAsync();
                }

            }
        }


        /// <summary>
        /// Update an existing violation type.
        /// </summary>
        /// <param name="violationtype"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<CommonViolationType>> UpdateType(CommonViolationType violationtype, Guid createdById)
        {
            var result = new ServiceResponse<CommonViolationType>();

            try
            {
                using (var scope = _commonCtx.Database.BeginTransaction())
                {
                    //update CommonViolationTypes table
                    var violationDetails = await _commonCtx.CommonViolationTypes.SingleOrDefaultAsync(a => a.Id == violationtype.Id);

                    violationDetails.UpdateUserId = createdById;
                    violationDetails.UpdateUtc = DateTime.Now;
                    violationDetails.Name = violationtype.Name;
                    violationDetails.Description = violationtype.Description;
                    violationDetails.Disabled = violationtype.Disabled;

                    _commonCtx.CommonViolationTypes.Update(violationDetails);
                    await _commonCtx.SaveChangesAsync();

                    ///Update violation types across all accounts
                    await UpdateAccountViolationType(violationDetails);

                    scope.Commit();

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating a violation type");
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }


        /// <summary>
        /// Updates violation type in the account context for every partition.
        /// </summary>
        public async Task UpdateAccountViolationType(CommonViolationType commonViolationType)
        {
            var allAccounts = await _commonCtx.CommonAccounts.AsNoTracking().Include(m => m.Partition).Where(m => m.Archived == false).ToListAsync();

            //Go through each account and add this CommonViolation to Violation
            foreach (var account in allAccounts)
            {

                using (var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(account.Partition.ConnectionString)))
                {
                    var violationType = await accountContext.ViolationTypes.Where(m => m.AccountId == account.Id && m.CommonViolationTypeId == commonViolationType.Id).SingleOrDefaultAsync();

                    if (violationType != null)
                    {
                        violationType.Name = commonViolationType.Name;
                        violationType.Description = commonViolationType.Description;

                        await accountContext.SaveChangesAsync();
                    }
                }

            }
        }

        #endregion

        #region Category

        /// <summary>
        /// create new violation category
        /// </summary>
        /// <param name="commonCategory"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<CommonViolationCategory>> CreateViolationCategory(CommonViolationCategory commonCategory, Guid createdById)
        {
            var result = new ServiceResponse<CommonViolationCategory>();

            try
            {
                using (var scope = _commonCtx.Database.BeginTransaction())
                {
                    commonCategory.CreateUserId = createdById;
                    commonCategory.UpdateUserId = createdById;

                    _commonCtx.CommonViolationCategories.Add(commonCategory);

                    await _commonCtx.SaveChangesAsync();

                    await CreateAccountViolationCategory(commonCategory);

                    scope.Commit();

                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating new Violation category");

                throw ex;
            }

            return result;
        }


        /// <summary>
        /// Creates a new new violation category in the account context for every partition.
        /// </summary>
        public async Task CreateAccountViolationCategory(CommonViolationCategory commonViolationCategory)
        {
            var allAccounts = await _commonCtx.CommonAccounts.AsNoTracking().Include(m => m.Partition).Where(m => m.Archived == false).ToListAsync();

            //Go through each account and add this CommonViolation to Violation
            foreach (var account in allAccounts)
            {

                using (var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(account.Partition.ConnectionString)))
                {
                    var accountViolationType = await accountContext.ViolationTypes.AsNoTracking().Where(m => m.AccountId == account.Id && m.CommonViolationTypeId == commonViolationCategory.TypeId).SingleOrDefaultAsync();

                    if (accountViolationType != null)
                    {
                        var created = DateTime.UtcNow;
                        var violationCategory = Mapper.Map<ViolationCategory>(commonViolationCategory);

                        violationCategory.TypeId = accountViolationType.Id;
                        violationCategory.AccountId = account.Id;
                        violationCategory.CreateUtc = created; //Setting created and UpdatedUtc to the exact same value will let us know if this record has every been changed. 
                        violationCategory.UpdateUtc = created;

                        accountContext.ViolationCategorys.Add(violationCategory);

                        await accountContext.SaveChangesAsync();
                    }
                }

            }
        }


        /// <summary>
        /// Update an existing violation category.
        /// </summary>
        /// <param name="Category"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<CommonViolationCategory>> UpdateViolationCategory(CommonViolationCategory Category, Guid createdById)
        {
            var result = new ServiceResponse<CommonViolationCategory>();

            try
            {
                using (var scope = _commonCtx.Database.BeginTransaction())
                {
                    //update CommonViolationTypes table
                    var violationDetails = await _commonCtx.CommonViolationCategories.SingleOrDefaultAsync(a => a.Id == Category.Id);

                    violationDetails.UpdateUserId = createdById;
                    violationDetails.UpdateUtc = DateTime.Now;
                    violationDetails.Name = Category.Name;
                    violationDetails.Description = Category.Description;
                    violationDetails.Disabled = Category.Disabled;
                    violationDetails.TypeId = Category.TypeId;

                    _commonCtx.CommonViolationCategories.Update(violationDetails);

                    await _commonCtx.SaveChangesAsync();
                    await UpdateAccountViolationCategory(violationDetails);

                    scope.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating a violation category");
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }


        /// <summary>
        /// Creates a new new violation category in the account context for every partition.
        /// </summary>
        public async Task UpdateAccountViolationCategory(CommonViolationCategory commonViolationCategory)
        {
            var allAccounts = await _commonCtx.CommonAccounts.AsNoTracking().Include(m => m.Partition).Where(m => m.Archived == false).ToListAsync();

            //Go through each account and add this CommonViolation to Violation
            foreach (var account in allAccounts)
            {
                using (var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(account.Partition.ConnectionString)))
                {
                    var accountViolationCategory = await accountContext.ViolationCategorys.Where(m => m.AccountId == account.Id && m.CommonCategoryId == commonViolationCategory.Id).SingleOrDefaultAsync();

                    if (accountViolationCategory != null)
                    {
                        var accountViolationType = await accountContext.ViolationTypes.AsNoTracking().Where(m => m.AccountId == account.Id && m.CommonViolationTypeId == commonViolationCategory.TypeId).SingleOrDefaultAsync();

                        if (accountViolationType != null)
                        {
                            accountViolationCategory.TypeId = accountViolationType.Id;
                            accountViolationCategory.Name = commonViolationCategory.Name;
                            accountViolationCategory.Description = commonViolationCategory.Description;

                            await accountContext.SaveChangesAsync();
                        }
                    }
                }

            }
        }

        #endregion

        #region Violation

        /// <summary>
        /// create new violation
        /// </summary>
        /// <param name="Violation"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<CommonViolation>> CreateViolation(CommonViolation Violation, Guid createdById)
        {
            var result = new ServiceResponse<CommonViolation>();

            try
            {
                using (var scope = _commonCtx.Database.BeginTransaction())
                {
                    Violation.CreateUserId = createdById;
                    Violation.UpdateUserId = createdById;

                    _commonCtx.CommonViolations.Add(Violation);

                    await _commonCtx.SaveChangesAsync();

                    //Add this violation to all accounts
                    await CreateAccountViolation(Violation);
                    scope.Commit();

                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating new Violation");

                throw ex;
            }

            return result;
        }


        /// <summary>
        /// Creates a new new violation in the account context for every partition.
        /// </summary>
        public async Task CreateAccountViolation(CommonViolation commonViolation)
        {
            var allAccounts = await _commonCtx.CommonAccounts.AsNoTracking().Include(m => m.Partition).Where(m => m.Archived == false).ToListAsync();

            //Go through each account and add this CommonViolation to Violation
            foreach (var account in allAccounts)
            {

                using (var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(account.Partition.ConnectionString)))
                {
                    var accountViolationCategory = await accountContext.ViolationCategorys.AsNoTracking().Where(m => m.AccountId == account.Id && m.CommonCategoryId == commonViolation.CategoryId).SingleOrDefaultAsync();

                    if (accountViolationCategory != null)
                    {
                        var created = DateTime.UtcNow;
                        var accountViolation = Mapper.Map<Violation>(commonViolation);

                        accountViolation.CategoryId = accountViolationCategory.Id;
                        accountViolation.CustomRequiredFields = commonViolation.RequiredFields;
                        accountViolation.CustomActions = commonViolation.Actions;
                        accountViolation.AccountId = account.Id;
                        accountViolation.CreateUtc = created; //Setting created and UpdatedUtc to the exact same value will let us know if this record has every been changed. 
                        accountViolation.UpdateUtc = created;

                        accountContext.Violations.Add(accountViolation);

                        await accountContext.SaveChangesAsync();
                    }
                }

            }
        }



        /// <summary>
        /// Update an existing violation.
        /// </summary>
        /// <param name="Violation"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<CommonViolation>> UpdateViolation(CommonViolation Violation, Guid createdById)
        {
            var result = new ServiceResponse<CommonViolation>();

            try
            {
                using (var scope = _commonCtx.Database.BeginTransaction())
                {
                    //update CommonViolation table
                    var violationDetails = await _commonCtx.CommonViolations.SingleOrDefaultAsync(a => a.Id == Violation.Id);

                    violationDetails.UpdateUserId = createdById;
                    violationDetails.UpdateUtc = DateTime.Now;
                    violationDetails.Name = Violation.Name;
                    violationDetails.Description = Violation.Description;
                    violationDetails.Disabled = Violation.Disabled;
                    violationDetails.Actions = Violation.Actions;
                    violationDetails.CategoryId = Violation.CategoryId;
                    violationDetails.HelpUrl = Violation.HelpUrl;
                    violationDetails.RequiredFields = Violation.RequiredFields;
                    violationDetails.ReminderMessage = Violation.ReminderMessage;
                    violationDetails.ReminderMinutes = Violation.ReminderMinutes;
                    //violationDetails.WarningQuizUrl = Violation.WarningQuizUrl;

                    _commonCtx.CommonViolations.Update(violationDetails);
                    await _commonCtx.SaveChangesAsync();

                    await UpdateAccountViolation(violationDetails);

                    scope.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating a violation");
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// Creates a new new violation category in the account context for every partition.
        /// </summary>
        public async Task UpdateAccountViolation(CommonViolation commonViolation)
        {
            var allAccounts = await _commonCtx.CommonAccounts.AsNoTracking().Include(m => m.Partition).Where(m => m.Archived == false).ToListAsync();

            //Go through each account and add this CommonViolation to Violation
            foreach (var account in allAccounts)
            {
                using (var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(account.Partition.ConnectionString)))
                {
                    var accountViolation = await accountContext.Violations.Where(m => m.AccountId == account.Id && m.CommonViolationId == commonViolation.Id).SingleOrDefaultAsync();

                    if (accountViolation != null)
                    {
                        var accountViolationCategory = await accountContext.ViolationCategorys.AsNoTracking().Where(m => m.AccountId == account.Id && m.CommonCategoryId == commonViolation.CategoryId).SingleOrDefaultAsync();

                        if (accountViolationCategory != null)
                        {
                            accountViolation.CategoryId = accountViolationCategory.Id;
                            accountViolation.Name = commonViolation.Name;
                            accountViolation.Description = commonViolation.Description;
                            accountViolation.HelpUrl = commonViolation.HelpUrl;
                            accountViolation.Actions = commonViolation.Actions;
                            accountViolation.RequiredFields = commonViolation.RequiredFields;
                            accountViolation.ReminderMinutes = commonViolation.ReminderMinutes;
                            //accountViolation.WarningQuizUrl = commonViolation.WarningQuizUrl;

                            await accountContext.SaveChangesAsync();
                        }
                    }
                }

            }
        }

        #endregion

        /// <summary>
        /// populate all violations in the account context for selected partition.
        /// </summary>
        public async Task PopulateViolationsForAccount(CommonAccount commonAccount, Guid createdById)
        {
            var AllViolationType = await _commonCtx.CommonViolationTypes.AsNoTracking().Where(m => m.Disabled == false).ToListAsync();
            var AllViolationCategory = await _commonCtx.CommonViolationCategories.AsNoTracking().Where(m => m.Disabled == false).ToListAsync();
            var AllViolation = await _commonCtx.CommonViolations.AsNoTracking().Where(m => m.Disabled == false).ToListAsync();

            //Go through each account and add this CommonViolation to Violation
            foreach (var violationType in AllViolationType)
            {
                using (var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString)))
                {
                    var created = DateTime.UtcNow;
                    var violationTypes = Mapper.Map<ViolationType>(violationType);

                    violationTypes.AccountId = commonAccount.Id;
                    violationTypes.CreateUtc = created; //Setting created and UpdatedUtc to the exact same value will let us know if this record has every been changed. 
                    violationTypes.UpdateUtc = created;
                    violationTypes.CommonViolationTypeId = violationType.Id;

                    accountContext.ViolationTypes.Add(violationTypes);

                    await accountContext.SaveChangesAsync();
                    await PurgeAccountViolation(commonAccount.Id);
                }

            }

            // add entries in violation category in the account context for this account
            foreach (var ViolationCategory in AllViolationCategory)
            {
                using (var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString)))
                {
                    var created = DateTime.UtcNow;
                    var accountViolationType = await accountContext.ViolationTypes.AsNoTracking().Where(m => m.AccountId == commonAccount.Id && m.CommonViolationTypeId == ViolationCategory.TypeId).SingleOrDefaultAsync();

                    if (accountViolationType != null)
                    {
                        var violationCategory = Mapper.Map<ViolationCategory>(ViolationCategory);

                        violationCategory.TypeId = accountViolationType.Id;
                        violationCategory.AccountId = commonAccount.Id;
                        violationCategory.CreateUtc = created; //Setting created and UpdatedUtc to the exact same value will let us know if this record has every been changed. 
                        violationCategory.UpdateUtc = created;
                        violationCategory.CommonCategoryId = ViolationCategory.Id;

                        accountContext.ViolationCategorys.Add(violationCategory);

                        await accountContext.SaveChangesAsync();
                        await PurgeAccountViolation(commonAccount.Id);
                    }
                }
            }

            // add entries in violations in the account context for this account
            foreach (var Violation in AllViolation)
            {
                using (var accountContext = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString)))
                {
                    var created = DateTime.UtcNow;
                    var accountViolationCategory = await accountContext.ViolationCategorys.AsNoTracking().Where(m => m.AccountId == commonAccount.Id && m.CommonCategoryId == Violation.CategoryId).SingleOrDefaultAsync();

                    if (accountViolationCategory != null)
                    {
                        var violation = Mapper.Map<Violation>(Violation);

                        violation.CategoryId = accountViolationCategory.Id;
                        violation.AccountId = commonAccount.Id;
                        violation.CreateUtc = created; //Setting created and UpdatedUtc to the exact same value will let us know if this record has every been changed. 
                        violation.UpdateUtc = created;
                        violation.CommonViolationId = Violation.Id;
                        violation.CustomActions = Violation.Actions;
                        violation.CustomRequiredFields = Violation.RequiredFields;

                        accountContext.Violations.Add(violation);

                        await accountContext.SaveChangesAsync();
                        await PurgeAccountViolation(commonAccount.Id);
                    }
                }
            }
        }


        /// <summary>
        /// Creates a new new violation category in the account context for every partition.
        /// </summary>
        public async Task<ServiceResponse<Violation>> UpdateUserAccountViolation(Violation Violationss, Guid createdById, AccountContext _accountCtx)
        {
            var result = new ServiceResponse<Violation>();

            try
            {
                using (var scope = _accountCtx.Database.BeginTransaction())
                {
                    //update CommonViolation table
                    var violationDetails = await _accountCtx.Violations.SingleOrDefaultAsync(a => a.Id == Violationss.Id);

                    violationDetails.UpdateUserId = createdById;
                    violationDetails.UpdateUtc = DateTime.Now;
                    violationDetails.CustomName = Violationss.CustomName;
                    violationDetails.CustomDescription = Violationss.CustomDescription;
                    violationDetails.CustomHelpUrl = Violationss.CustomHelpUrl;
                    violationDetails.Code = Violationss.Code;
                    violationDetails.CustomActions = Violationss.CustomActions;
                    violationDetails.CustomRequiredFields = Violationss.CustomRequiredFields;
                    violationDetails.Fee = Violationss.Fee;
                    violationDetails.ReminderMessage = Violationss.ReminderMessage;
                    violationDetails.ReminderMinutes = Violationss.ReminderMinutes;

                    _accountCtx.Violations.Update(violationDetails);

                    await _accountCtx.SaveChangesAsync();

                    scope.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating a violation");
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }


        public async Task<List<CachedAccountViolations>> GetCachedAccountViolations(Guid accountId)
        {

            //First we need to get the correct cache key
            var cacheKey = WebCacheKey.AccountViolations(accountId);

            //Then we need to set the cache expiration time. 
            var expiry = TimeSpan.FromMinutes(15);

            var cachedViolations = await _cache.GetWithSlidingExpirationAsync<List<CachedAccountViolations>>(cacheKey, expiry);

            if (cachedViolations == null)
            {
                var violations = await _accountCtx.Violations.ForAccount(accountId)
                .Include(m => m.Category).ThenInclude(m => m.Type)
                .ToListAsync();
                cachedViolations = Mapper.Map<List<CachedAccountViolations>>(violations);

                // Found it. Add it to the cache.
                await _cache.SetAsync(cacheKey, cachedViolations, expiry);
            }

            return cachedViolations;

        }

        protected async Task PurgeAccountViolation(Guid accountId)
        {
            var cacheKey = WebCacheKey.AccountViolations(accountId);
            await _cache.RemoveAsync(cacheKey);
        }

        public async Task<ServiceResponse<ViolationQuestion>> SaveQuestion(Guid accountId, Guid violationId, Guid questionId, string question, bool IsPublic, ViolationQuestionType type, string choices, Guid createdById)
        {
            var result = new ServiceResponse<ViolationQuestion>();

            try
            {
                using (var scope = _accountCtx.Database.BeginTransaction())
                {
                    //update CommonViolation table
                    if (questionId != Guid.Empty)
                    {
                        var QuestionDetails = await _accountCtx.ViolationQuestions.SingleOrDefaultAsync(a => a.Id == questionId);
                        if (QuestionDetails != null)
                        {

                            //  CitationDetails.UpdateUserId = createdById;
                            QuestionDetails.UpdateUtc = DateTime.Now;
                            QuestionDetails.Question = question;
                            QuestionDetails.Type = type;
                            QuestionDetails.Choices = choices;
                            QuestionDetails.IsRequired = IsPublic;

                            _accountCtx.ViolationQuestions.Update(QuestionDetails);
                        }
                    }
                    else
                    {
                        var questionCount = await _accountCtx.ViolationQuestions.Where(a => a.ViolationId == violationId).CountAsync();
                        ViolationQuestion ViolationDetails = new ViolationQuestion
                        {
                            ViolationId = violationId,
                            AccountId = accountId,
                            Question = question,
                            CreateUserId = createdById,
                            UpdateUserId = createdById,
                            Order = questionCount,
                            Type = type,
                            Choices = choices,
                            IsRequired = IsPublic
                        };
                        _accountCtx.ViolationQuestions.Add(ViolationDetails);

                    }

                    await _accountCtx.SaveChangesAsync();

                    scope.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating a citation");
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;

        }


    }
}
