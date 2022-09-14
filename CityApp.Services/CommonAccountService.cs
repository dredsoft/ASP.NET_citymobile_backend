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
    public class CommonAccountService : ICustomService
    {

        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
        private static readonly object _randomLock = new object();
        private readonly CommonContext _commonCtx;
        private readonly RedisCache _cache;
        private readonly ViolationService _violationSvc;

        private static readonly ILogger _logger = Log.Logger.ForContext<CommonAccountService>();

        public CommonAccountService(CommonContext commonCtx, RedisCache cache, ViolationService violationSvc)
        {
            _commonCtx = commonCtx;
            _cache = cache;
            _violationSvc = violationSvc;
        }

        /// <summary>
        /// Get Cached Accounts
        /// </summary>
        /// <returns></returns>
        public async Task<List<CachedAccount>> GetAccountsAsync()
        {
            //First we need to get the correct cache key
            var cacheKey = WebCacheKey.CommonAccounts;

            //Then we need to set the cache expiration time. 
            var expiry = TimeSpan.FromHours(24);

            var cachedAccounts = await _cache.GetWithSlidingExpirationAsync<List<CachedAccount>>(cacheKey, expiry);

            if (cachedAccounts == null)
            {
                var commonAccounts = await _commonCtx.CommonAccounts.AsNoTracking().ToListAsync();
                cachedAccounts = Mapper.Map<List<CachedAccount>>(commonAccounts);

                // Found it. Add it to the cache.
                await _cache.SetAsync(cacheKey, cachedAccounts, expiry);
            }

            return cachedAccounts;
        }


        private static readonly TimeSpan _commonAccountCacheExpiry = TimeSpan.FromHours(24);

        /// <summary>
        /// Look for <see cref="Data.Models.Common.CommonAccount" /> in cache. If found, return it. If not found,
        /// load it from the database and cache it.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        public async Task<CachedAccount> GetCachedAccountAsync(long accountNumber)
        {
            var cacheKey = WebCacheKey.CommonAccount(accountNumber);

            // Try to pull it from the cache first.
            var cachedAccount = await _cache.GetWithSlidingExpirationAsync<CachedAccount>(cacheKey, _commonAccountCacheExpiry);
            if (cachedAccount != null)
            {
                return cachedAccount;
            }

            // Not in cache. Check the database.
            var commonAccount = await _commonCtx.CommonAccounts.AsNoTracking()
                    .Include(ca => ca.Partition)
                    .Include(ca => ca.Settings)
                    .Include(ca => ca.City)
                    .Include(ca => ca.CommonAccountViolationTypes)
                    .SingleOrDefaultAsync(ca => ca.Number == accountNumber);

            if (commonAccount == null)
            {
                // Account not found.
                return null;
            }

            cachedAccount = Mapper.Map<CachedAccount>(commonAccount);

            var commonViolationTypes = await _commonCtx.CommonAccountViolationTypes.Where(m => m.AccountId == commonAccount.Id).Select(m => m.ViolationTypeId).ToListAsync();
            var accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString));
            var violationTypes = await accountCtx.ViolationTypes.ForAccount(commonAccount.Id).Where(m => commonViolationTypes.Contains(m.CommonViolationTypeId)).ToListAsync();
                      

            cachedAccount.ViolationTypes = Mapper.Map<ViolationTypeModel[]>(violationTypes);
            
            
            // Found it. Add it to the cache.
            await _cache.SetAsync(cacheKey, cachedAccount, _commonAccountCacheExpiry);

            return cachedAccount;
        }


        /// <summary>
        /// Creates a new Account in the Common and Account partitions.
        /// Also creates a user in the Account Partition if one doesn't exist.
        /// </summary>
        /// <param name="commonAccount"></param>
        /// <param name="user"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<CommonAccount>> CreateAccount(CommonAccount commonAccount, Guid createdById, List<AccountViolationType> AccViolationType, long CitationCounter)
        {
            var result = new ServiceResponse<CommonAccount>();
            try
            {
                using (var scope = _commonCtx.Database.BeginTransaction())
                {
                    //Get the correct account database based on the partition that was chosen for the account.
                    var accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString));

                    if (accountCtx == null)
                    {
                        _logger.Error($"Account context does not exists for partition: {Cryptography.Decrypt(commonAccount.Partition.ConnectionString)}");

                        throw new Exception("Account context does not exists");
                    }

                    //Generate a random account number
                    commonAccount.Number = await GetNextAccountNumberAsync();
                    commonAccount.CreateUserId = createdById;
                    commonAccount.UpdateUserId = createdById;
                   
                    //Create empty account settings
                    commonAccount.Settings = new CommonAccountSettings { };
                    commonAccount.Settings.CreateUserId = createdById;
                    commonAccount.Settings.UpdateUserId = createdById;

                    _commonCtx.CommonAccounts.Add(commonAccount);

                    await _commonCtx.SaveChangesAsync();


                    //Update the count on the partition to let us know how many account reside on one database.
                    commonAccount.Partition.Occupancy = commonAccount.Partition.Occupancy + 1;
                    _commonCtx.Partitions.Update(commonAccount.Partition);


                    //If a Owner is selected, create a user account assosication.
                    if (commonAccount.OwnerUserId != null)
                    {
                        CommonUserAccount userAccount = new CommonUserAccount();
                        userAccount.CreateUserId = createdById;
                        userAccount.AccountId = commonAccount.Id;
                        userAccount.UpdateUserId = createdById;
                        userAccount.UserId = commonAccount.OwnerUserId;
                        userAccount.Permissions = (AccountPermissions)Enum.GetValues(typeof(AccountPermissions)).Cast<int>().Sum();

                        _commonCtx.UserAccounts.Add(userAccount);
                        await _commonCtx.SaveChangesAsync();

                        var commonUser = await _commonCtx.Users.SingleAsync(m => m.Id == commonAccount.OwnerUserId);

                        //Add any new users to acconts
                        await SaveAccountUser(commonUser, createdById);

                    }

                    //Add Account in account context
                    //The Ids for the Account in AccountContext and CommonAccount in CommonContext are going to be exactly the same.
                    accountCtx.Accounts.Add(new Account { Id = commonAccount.Id, Name = commonAccount.Name, CreateUserId = createdById, UpdateUserId = createdById });

                    await accountCtx.SaveChangesAsync();


                    foreach (var Type in AccViolationType)
                    {
                        if (Type.IsCheckedViolation == true)
                        {
                            CommonAccountViolationType AccountViolationType = new CommonAccountViolationType();
                            AccountViolationType.CreateUserId = createdById;
                            AccountViolationType.AccountId = commonAccount.Id;
                            AccountViolationType.UpdateUserId = createdById;
                            AccountViolationType.ViolationTypeId = Type.TypeId;

                            _commonCtx.CommonAccountViolationTypes.Add(AccountViolationType);
                        }
                    }

                    await _commonCtx.SaveChangesAsync();

                    await _violationSvc.PopulateViolationsForAccount(commonAccount, createdById);

                    Counter count = new Counter()
                    {
                        AccountId = commonAccount.Id,
                        Name = "Citations",
                        NextValue = CitationCounter,
                        CreateUserId = commonAccount.OwnerUserId.Value,
                        UpdateUserId = commonAccount.OwnerUserId.Value
                    };
                    accountCtx.Counters.Add(count);

                    await accountCtx.SaveChangesAsync();

                    scope.Commit();
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating new Account");


                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Create or Update a user information across all partiations
        /// </summary>
        /// <param name="commonUser"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public async Task SaveAccountUser(CommonUser commonUser, Guid createdById, bool isAdmin = false)
        {
            //Get all accounts that this user belongs to
            var accounts = await _commonCtx.UserAccounts.Include(m => m.Account).ThenInclude(m => m.Partition).Where(m => m.UserId == commonUser.Id).ToListAsync();

            //Go through each account and create or update the user's informatoin
            foreach (var account in accounts)
            {

                //generate an account context for this account
                var accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(account.Account.Partition.ConnectionString));

                //check to see if this user exists in the account
                var user = await accountCtx.AccountUsers.SingleOrDefaultAsync(m => m.Id == commonUser.Id);

                //If the user doesn't exit, create them.
                if (user == null)
                {

                    user = new AccountUser
                    {
                        Id = commonUser.Id,  //IMPORTANT: CommonUser.Id and AccountUser.Id must always match
                        FirstName = commonUser.FirstName,
                        MiddleName = commonUser.MiddleName,
                        LastName = commonUser.LastName,
                        Email = commonUser.Email,
                        ProfileImageKey = commonUser.ProfileImageKey,
                        CreateUserId = createdById,
                        UpdateUserId = createdById
                    };

                    accountCtx.AccountUsers.Add(user);
                }
                else
                {
                    user.FirstName = commonUser.FirstName;
                    user.MiddleName = commonUser.MiddleName;
                    user.LastName = commonUser.LastName;
                    user.UpdateUserId = createdById;
                    user.UpdateUtc = DateTime.UtcNow;
                    user.ProfileImageKey = commonUser.ProfileImageKey;
                }
                await accountCtx.SaveChangesAsync();
            }
        }

        public async Task<long> GetNextAccountNumberAsync()
        {
            var currentMaxNumber = await _commonCtx.CommonAccounts
                .Select(ca => ca.Number)
                .MaxAsync();

            if (currentMaxNumber < 1000)
            {
                currentMaxNumber = 1000;
            }

            return currentMaxNumber + GetRandomOffset();
        }

        /// <summary>
        /// We don't want people to look at our numbers and be able to guess the number of customers we have, or to be
        /// able to sequentially guess each customer's account number, so generate a random offset to add to new 
        /// account numbers.
        /// </summary>
        /// <returns></returns>
        private long GetRandomOffset()
        {
            // Random is not thread-safe.
            lock (_randomLock)
            {
                // The upper bound is exclusive, so this will return a random number between 1 and 9, inclusive.
                return _random.Next(1, 10);
            }
        }


        /// <summary>
        /// Update an existing Account in the Common and Account partitions.
        /// Also update a user in the Account Partition.
        /// </summary>
        /// <param name="commonAccount"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<CommonAccount>> UpdateAccount(CommonAccount commonAccount, Guid createdById, int AccountNumber, List<AccountViolationType> AccViolationType)
        {
            var result = new ServiceResponse<CommonAccount>();

            try
            {
                using (var scope = _commonCtx.Database.BeginTransaction())
                {
                    //update common account table
                    var accountDetail = await _commonCtx.CommonAccounts.Include(m => m.Partition).Include(m => m.Settings).SingleOrDefaultAsync(a => a.Id == commonAccount.Id);


                    var accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(accountDetail.Partition.ConnectionString));


                    accountDetail.UpdateUserId = createdById;
                    accountDetail.UpdateUtc = DateTime.Now;
                    accountDetail.Name = commonAccount.Name;
                    accountDetail.OwnerUserId = commonAccount.OwnerUserId;
                    accountDetail.StorageBucketName = commonAccount.StorageBucketName;
                    accountDetail.CitationWorkflow = commonAccount.CitationWorkflow;
                    accountDetail.Features = commonAccount.Features;

                    _commonCtx.CommonAccounts.Update(accountDetail);
                    await _commonCtx.SaveChangesAsync();


                    //update accounts table from selected partition
                    var AccountsDetail = await accountCtx.Accounts.SingleOrDefaultAsync(a => a.Id == commonAccount.Id);
                    AccountsDetail.Name = commonAccount.Name;
                    AccountsDetail.UpdateUserId = createdById;
                    AccountsDetail.UpdateUtc = DateTime.UtcNow;

                    accountCtx.Accounts.Update(AccountsDetail);
                    await accountCtx.SaveChangesAsync();

                    //update user accounts table 
                    if (commonAccount.OwnerUserId != null)
                    {
                        //Check to see if the owner exists in the User Account Table. 
                        var userAccount = await _commonCtx.UserAccounts.SingleOrDefaultAsync(a => a.AccountId == commonAccount.Id && a.UserId == commonAccount.OwnerUserId);
                        if (userAccount == null)
                        {
                            CommonUserAccount NewuserAccount = new CommonUserAccount();
                            NewuserAccount.CreateUserId = createdById;
                            NewuserAccount.AccountId = commonAccount.Id;
                            NewuserAccount.UpdateUserId = createdById;
                            NewuserAccount.UserId = commonAccount.OwnerUserId;
                            NewuserAccount.Permissions = (AccountPermissions)Enum.GetValues(typeof(AccountPermissions)).Cast<int>().Sum();

                            _commonCtx.UserAccounts.Add(NewuserAccount);

                            await _commonCtx.SaveChangesAsync();
                            var commonUser = await _commonCtx.Users.SingleAsync(m => m.Id == commonAccount.OwnerUserId);

                            //Add any new users to acconts
                            await SaveAccountUser(commonUser, createdById);
                        }
                    }


                    //Remove the current violation types
                    var AccountViolationType = await _commonCtx.CommonAccountViolationTypes.Where(av => av.AccountId == commonAccount.Id).ToListAsync();

                    _commonCtx.CommonAccountViolationTypes.RemoveRange(AccountViolationType);
                    await _commonCtx.SaveChangesAsync();


                    //Add new violatoin types from the page
                    foreach (var Types in AccViolationType)
                    {
                        if (Types.IsCheckedViolation == true)
                        {
                            CommonAccountViolationType AcntViolationType = new CommonAccountViolationType();
                            AcntViolationType.CreateUserId = createdById;
                            AcntViolationType.AccountId = commonAccount.Id;
                            AcntViolationType.UpdateUserId = createdById;
                            AcntViolationType.ViolationTypeId = Types.TypeId;

                            _commonCtx.CommonAccountViolationTypes.Add(AcntViolationType);
                            await _commonCtx.SaveChangesAsync();
                        }
                    }
                    scope.Commit();

                    //Purge common accounts cache
                    var cacheKey = WebCacheKey.CommonAccount(accountDetail.Number);
                    await _cache.RemoveAsync(cacheKey);
                }


            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating an Account");

                throw ex;
            }

            return result;
        }

        public async Task<ServiceResponse<Violation>> UpdateDisableStatus(bool Disabled, Guid ViolationId, AccountContext _accountCtx)
        {
            var result = new ServiceResponse<Violation>();

            try
            {
                using (var scope = _commonCtx.Database.BeginTransaction())
                {

                    //update common account table
                    var violationDetails = await _accountCtx.Violations.SingleOrDefaultAsync(a => a.Id == ViolationId);


                    violationDetails.Disabled = Disabled;

                    _accountCtx.Violations.Update(violationDetails);
                    await _accountCtx.SaveChangesAsync();

                    scope.Commit();
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating an Account");

                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Update User CommonAccount Profile
        /// </summary>
        /// <param name="commonAccount"></param>
        /// <param name="createdById"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<CommonAccount>> UpdateSettingAccount(CommonAccount commonAccount, Guid createdById)
        {
            var result = new ServiceResponse<CommonAccount>();

            try
            {
                using (var scope = _commonCtx.Database.BeginTransaction())
                {                                   

                    _commonCtx.CommonAccounts.Update(commonAccount);
                    await _commonCtx.SaveChangesAsync();


                    //update CommonAccountSettings table
                    var accountSettingDetail = await _commonCtx.CommonAccountSettings.SingleOrDefaultAsync(a => a.Id == commonAccount.Id);
                    accountSettingDetail.UpdateUserId = createdById;
                    accountSettingDetail.UpdateUtc = DateTime.UtcNow;
                    _commonCtx.CommonAccountSettings.Update(accountSettingDetail);
                    await _commonCtx.SaveChangesAsync();

                    //update accounts table from selected partition
                    var accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString));

                    var AccountsDetail = await accountCtx.Accounts.SingleOrDefaultAsync(a => a.Id == commonAccount.Id);
                    AccountsDetail.Name = commonAccount.Name;
                    AccountsDetail.UpdateUserId = createdById;
                    AccountsDetail.UpdateUtc = DateTime.UtcNow;

                    accountCtx.Accounts.Update(AccountsDetail);
                    await accountCtx.SaveChangesAsync();  
                    
                    scope.Commit();
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating an Account");

                throw ex;
            }

            return result;
        }




        public async Task<string> Test(string Email)
        {
            return Email;

        }
    }
}
