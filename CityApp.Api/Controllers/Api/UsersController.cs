using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using CityApp.Api.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using CityApp.Data;
using CityApp.Common.Caching;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using CityApp.Common.Extensions;
using CityApp.Services;
using CityApp.Api.Models.Citation;
using CityApp.Common.Utilities;
using CityApp.Data.Models;

namespace CityApp.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Users")]
    public class UsersController : BaseApiController
    {
        private readonly CommonAccountService _commonAccountSvc;
        private readonly CommonUserService _commonUserSvc;
        private AccountContext _accountCtx;
        private readonly PushNotification _PushSvc;
        private static readonly ILogger _logger = Log.Logger.ForContext<CitationsController>();

        public UsersController(CommonContext commonContext, RedisCache cache, IOptions<AppSettings> appSettings, CommonAccountService commonAccountSvc, CommonUserService commonUserSvc,
            AccountContext accountCtx, PushNotification PushSvc) :
            base(commonContext, cache, appSettings)
        {
            _accountCtx = accountCtx;
            _commonAccountSvc = commonAccountSvc;
            _commonUserSvc = commonUserSvc;
            _PushSvc = PushSvc;
        }

        /// <summary>
        /// Get AccountUser Information for a specific user. 
        /// Only System Admin can get data for other users
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Route("{Id}/Accounts")]
        public async Task<IActionResult> Accounts(Guid Id)
        {
            if (LoggedInUser.Id != Id && LoggedInUser.Permission != Data.Enums.SystemPermissions.Administrator)
            {
                return Ok(new APIResponse<string> { Success = false, Message = "You are not Authorized to view this users Accounts.", Errors = new List<Error> { new Error { Code = Common.Enums.ErrorCode.Forbidden } } });
            }
            else
            {
                var userAccounts = await CommonContext.UserAccounts.AsNoTracking().Include(m => m.Account).Where(m => m.UserId == Id).ToListAsync();

                var commonUserAccounts = Mapper.Map<List<CommonUserAccountModel>>(userAccounts);

                var responseModel = new APIResponse<List<CommonUserAccountModel>>()
                {
                    Data = commonUserAccounts
                };
                if (commonUserAccounts.Count == 0)
                {
                    responseModel.Message = "Your login is not associated with any accounts.";
                }


                return Ok(responseModel);
            }
        }

        [Route("{Id}/GetUserProfile")]
        public async Task<IActionResult> Get(Guid Id)
        {
            var response = new APIResponse<UserProfileModel>() { Success = false };
            var user = await _commonUserSvc.GetUserProfile(Id);
            if (user != null)
            {
                var userResponse = Mapper.Map<UserProfileModel>(user);
                if (!string.IsNullOrWhiteSpace(user.ProfileImageKey))
                {
                    userResponse.ProfileImageUrl = AppSettings.AmazonS3Url + user.ProfileImageKey;
                }

                response.Data = userResponse;
                response.Success = true;
            }


            //Get a list of Accounts for a user
            var accountsForaUser = await CommonContext.UserAccounts.Include(m => m.Account).ThenInclude(m => m.Partition).Where(m => m.UserId == Id).ToListAsync();

            var citations = new List<Citation>();

            //Loop through each AccountContext for this user and get a list of Citations
            foreach (var commonAccount in accountsForaUser)
            {
                //Get the correct account database based on the partition that was chosen for the account.
                var accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Account.Partition.ConnectionString));

                var citationsForAccount = await accountCtx.Citations.Where(m => m.CreateUserId == Id).ToListAsync();
                citations.AddRange(citationsForAccount);

            }

            return Ok(response);
        }

        [Route("{Id}/UpdateProfile")]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody]UserModel model)
        {

            var response = new APIResponse<UserModel>();
            if (ModelState.IsValid)
            {
                // if user is trying to update someone else profile.
                if (User.GetJWTLoggedInUserId() != model.Id)
                {
                    response.Success = false;
                    response.Errors.Add(new Error { Code = 0, Message = "Invalid ID" });
                    return Ok(response);
                }



                var userAlreadyExists = await CommonContext.Users.AsNoTracking().AnyAsync(q => q.Email.ToLower() == model.Email.ToLower() && q.Id != model.Id);
                if (userAlreadyExists)
                {
                    // This isn't a security risk because we've verified the email address already
                    response.Success = false;
                    response.Errors.Add(new Error { Code = 0, Message = "A user has already verified that email address." });
                    return Ok(response);
                }
                var user = await CommonContext.Users.SingleAsync(m => m.Id == model.Id);
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.ProfileImageKey = model.profileImageKey;


                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    user.SetPassword(model.Password);
                }

                CommonContext.Users.Update(user);
                await CommonContext.SaveChangesAsync();

                //Update user information in all other accounts
                await _commonAccountSvc.SaveAccountUser(user, model.Id);

                //purge the user cache
                var cacheKey = WebCacheKey.LoggedInUser(model.Id);
                await _cache.RemoveAsync(cacheKey);

                response.Message = "Your profile has been updated";

            }
            else
            {
                response.Success = false;
                response.Errors.AddRange(ModelState.ToErrors());
            }

            return Ok(response);
        }


        [HttpPost("{Id}/Citations")]
        public async Task<IActionResult> Citations([FromBody] CitationModel model, Guid Id)
        {
            var response = new APIResponse<CitationModel>() { Success = true };

            try
            {

                var currentPageNum = model.Page;
                var offset = (model.PageSize * currentPageNum) - model.PageSize;

                //Get a list of Accounts for a user
                var userAccount = await CommonContext.UserAccounts.Include(m => m.Account).ThenInclude(m => m.Partition).Where(m => m.UserId == Id).ToListAsync();

                var citations = new List<Citation>();

                //Loop through each AccountContext for this user and get a list of Citations
                foreach (var commonAccount in userAccount)
                {
                    //Get the correct account database based on the partition that was chosen for the account.
                    var accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Account.Partition.ConnectionString));

                    var citationsForAccount = accountCtx.Citations.Include(x => x.Violation)
                            .Include(x => x.AssignedTo)
                            .Include(x => x.Comments).Include(m => m.CreateUser)
                            .Include(x => x.Attachments).ThenInclude(m => m.Attachment)
                            .OrderByDescending(x => x.CreateUtc)
                            .Where(m => m.AccountId == commonAccount.AccountId).AsQueryable();

                    citations.AddRange(citationsForAccount);

                }


                var totalQueryCount = citations.Count();
                var results = citations.Skip(offset).Take(model.PageSize).ToList();
                var data = Mapper.Map<List<CitationListModel>>(results);
                model.CitationList = data;
                model.CurrentPage = currentPageNum;
                model.ItemsPerPage = model.PageSize;
                model.TotalItems = totalQueryCount;
                response.Data = model;
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return Ok(response);
            }
        }

        [HttpPost("{Id}/ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword model, Guid Id)
        {
            var response = new APIResponse<ChangePassword>() { Success = true };
            if (ModelState.IsValid)
            {
                var user = await CommonContext.Users.SingleOrDefaultAsync(m => m.Id == Id);
                if (user != null)
                {
                    if (user.CheckPassword(model.OldPassword))
                    {
                        user.SetPassword(model.NewPassword);
                        CommonContext.Users.Update(user);
                        await CommonContext.SaveChangesAsync();
                        response.Message = "Password Changed Successfully.";
                    }
                    else
                    {
                        response.Success = false;
                        response.Errors.Add(new Error() { Code = 0, Message = "Incorrect Password." });
                    }
                }
            }
            else
            {
                response.Success = false;
                response.Errors.AddRange(ModelState.ToErrors());
            }

            return Ok(response);
        }
        /// <summary>
        ///Send Push Notification For testing purpose
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{Id}/SendPushNotification")]
        public async Task<IActionResult> SendPushNotification([FromBody] UserDevice model)
        {
            var response = new APIResponse<UserDevice>() { Success = true };
            try
            {
                //DeviceToken = "77df959ac96c11b305ec7dc9f69bea5caf3134fddf9661407b142e11a692a156"
                if (model.Device.Count > 0)
                {
                    await _PushSvc.APNS_Send(model.Device, model.Message);
                    response.Data = model;
                    response.Message = "Notification Sent!";
                }
                else {
                    response.Success = false;
                    response.Data = model;
                    response.Message = "Device Token Required!";

                }
            }
            catch (Exception ex)
            {

                response.Success = false;
                response.Message = ex.Message;
            }

            return Ok(response);

        }
    }
}
