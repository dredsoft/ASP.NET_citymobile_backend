using AutoMapper;
using CityApp.Common.Caching;
using CityApp.Common.Extensions;
using CityApp.Common.Models;
using CityApp.Common.Utilities;
using CityApp.Data;
using CityApp.Data.Enums;
using CityApp.Data.Models;
using CityApp.Services;
using CityApp.Services.Models;
using CityApp.Web.Constants;
using CityApp.Web.Filters;
using CityApp.Web.Models;
using CityApp.Web.Models.AccountSettings;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Web.Controllers
{
    [RequiresPermission(AccountPermissions.AccountAdministrator)]
    public class AccountSettingsController : AccountBaseController
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

        public AccountSettingsController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache redisCache, IOptions<AppSettings> appSettings, AccountContext accountCtx, CommonUserService commonUserSvc, MailService mailSvc, ViolationService violationSvc, FileService fileService, CommonAccountService commonAccountSvc, PushNotification pushSvc)
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

        #region Invite
        public async Task<IActionResult> Invite(Guid? VendorId, bool IsVendor = false)
        {
            var model = new AccountUserViewModel();
            model.IsVendor = IsVendor;
            if (IsVendor)
            {
                model.systemPermission = SystemPermissions.Vendor;
            }

            model.VendorId = VendorId;
            await PopulateDropDownVendors(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invite(AccountUserViewModel model)
        {

            if (ModelState.IsValid)
            {
                //Check to see if this user exists
                var existingUser = await CommonContext.UserAccounts.AsNoTracking()
                    .Include(m => m.User)
                    .Where(m => m.AccountId == CommonAccount.Id)
                    .Where(m => m.User.Email.ToLower() == model.Email.ToLower())
                    .AnyAsync();

                if (existingUser)
                {
                    ModelState.AddModelError(nameof(model.Email), "This user already exists");
                }
                else
                {
                    //TODO: Now extract the permissions, first name, last name, email and tokenize them with a date.
                    var permissions = (int)model.Permissions;
                    var systemPermission = (int)model.systemPermission;

                    //TODO: Create a link with the tokenized string to User/RegisterInvitation.  
                    //Send the email template CityApp.Common/EmailTemplates/UserInvitation.

                    var UserName = model.FirstName + " " + model.LastName;
                    var TimeStamp = string.Format("{0:dd-MMM-yyy hh:mm:ss}", DateTime.Now);

                    var emailtoken = Cryptography.Encrypt(model.Email + "/" + Convert.ToString(permissions) + "/" + CommonAccount.Id + "/" + UserName + "/" + TimeStamp + "/" + model.VendorId + "/" + Convert.ToString(systemPermission));
                    var callbackUrl = Url.Action("AcceptInvite", "User", new { code = emailtoken });


                    bool result = await _mailSvc.SendInvitationEmail(UserName, model.Email, callbackUrl);

                    return RedirectToAction("Users");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Users(AccountUserListViewModel model)
        {

            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;
            //Convert list to generic IEnumerable using AsQueryable          
            var user = CommonContext.UserAccounts
                .Include(m => m.User)
                .Where(m => m.AccountId == CommonAccount.Id).AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Searchstring))
            {
                user = user.Where(s =>
                            s.User.FirstName.ToLower().Contains(model.Searchstring.ToLower())
                            || s.User.LastName.ToLower().Contains(model.Searchstring.ToLower())
                            || s.User.Email.ToLower().Contains(model.Searchstring.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(model.UserType))
            {
                user = user.Where(x => x.User.Permission == (SystemPermissions)Enum.Parse(typeof(SystemPermissions), model.UserType));
            }


            switch (model.SortOrder)
            {
                case "FullName":
                    if (model.SortDirection == "DESC")
                        user = user.OrderByDescending(x => x.User.FullName);
                    else
                        user = user.OrderBy(x => x.User.FullName);
                    break;
                case "Email":
                    if (model.SortDirection == "DESC")
                        user = user.OrderByDescending(x => x.User.Email);
                    else
                        user = user.OrderBy(x => x.User.Email);
                    break;
                case "Status":
                    if (model.SortDirection == "DESC")
                        user = user.OrderByDescending(x => x.Disabled);
                    else
                        user = user.OrderBy(x => x.Disabled);
                    break;

                case "UserType":
                    if (model.SortDirection == "DESC")
                        user = user.OrderByDescending(x => x.User.Permission);
                    else
                        user = user.OrderBy(x => x.User.Permission);
                    break;

                default:
                    user = user.OrderByDescending(x => x.User.FullName);
                    break;
            }

            model.Paging.TotalItems = await user.CountAsync();

            model.AccountUserList = Mapper.Map<List<AccountUserListItem>>(await user.Skip(offset).Take(model.PageSize).ToListAsync());

            model.Paging.CurrentPage = currentPageNum;
            model.Paging.ItemsPerPage = model.PageSize;

            return View(model);
        }

        public async Task<IActionResult> Edit(Guid Id)
        {
            AccountUserViewModelEdit model = new AccountUserViewModelEdit();

            var existingUser = await CommonContext.UserAccounts.Include(x => x.User)
                .Where(m => m.UserId == Id && m.AccountId == CommonAccount.Id).SingleOrDefaultAsync();

            model.Id = existingUser.Id;
            model.FirstName = existingUser.User.FirstName;
            model.LastName = existingUser.User.LastName;
            model.Permissions = existingUser.Permissions;
            model.UserId = existingUser.User.Id;
            model.Email = existingUser.User.Email;
            model.Disabled = existingUser.Disabled;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccountUserViewModelEdit model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await CommonContext.UserAccounts.Include(x => x.User)
                .Where(m => m.UserId == model.UserId && m.AccountId == CommonAccount.Id).SingleOrDefaultAsync();

                if (existingUser == null)
                {
                    throw new Exception($"User cannot be found. UserId:{model.UserId}");
                }

                if (User.GetLoggedInUserId().Value == model.UserId && !model.Permissions.HasFlag(AccountPermissions.AccountAdministrator))
                {
                    ModelState.AddModelError(string.Empty, "You cannot remove the Account Administrator permission");
                    model.FirstName = existingUser.User.FirstName;
                    model.LastName = existingUser.User.LastName;
                    model.Email = existingUser.User.Email;
                    return View(model);
                }

                if (User.GetLoggedInUserId().Value == model.UserId && model.Disabled)
                {
                    ModelState.AddModelError(string.Empty, "You cannot disable your own account");
                    model.FirstName = existingUser.User.FirstName;
                    model.LastName = existingUser.User.LastName;
                    model.Email = existingUser.User.Email;
                    return View(model);
                }

                existingUser.Permissions = model.Permissions;
                existingUser.Disabled = model.Disabled;
                existingUser.UpdateUserId = LoggedInUser.Id;
                existingUser.UpdateUtc = DateTime.UtcNow;

                using (var tx = CommonContext.Database.BeginTransaction())
                {
                    CommonContext.UserAccounts.Update(existingUser);
                    await CommonContext.SaveChangesAsync();
                    tx.Commit();

                    var cacheKey = WebCacheKey.CommonUserAccount(CommonAccount.Number, existingUser.UserId.Value);
                    await _cache.RemoveAsync(cacheKey);
                }

                return RedirectToAction("Users");

            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage(string message, string recipient, string displayName)
        {
            var result = new ServiceResponse<AccountUserViewModelEdit>() { Success = true };
            try
            {
                var currentUser = await _accountCtx.AccountUsers.Where(x => x.Id == User.GetLoggedInUserId().Value).SingleOrDefaultAsync();
                bool mailResult = await _mailSvc.SendMessageToReceipt(message, recipient, displayName, currentUser.DisplayName);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return Json(result);
        }

        #endregion

        #region Violation
        /// <summary>
        /// get list of all violation related to account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> Violations(AccountViolationListViewModel model)
        {
            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            //get commom violation type
            var vioTypeId = await CommonContext.CommonAccountViolationTypes.AsNoTracking()
                                .Where(s => s.AccountId.Equals(CommonAccount.Id)).Select(m => m.ViolationTypeId).ToListAsync();

            //get Violation that are disabled or that is related to passed accountid
            var violations = _accountCtx.Violations
                .Include(m => m.Category)
                .Include(m => m.Category.Type)
                .ForAccount(CommonAccount.Id)
                .Where(m => !m.Disabled)
                .AsQueryable();

            //Filter the violation by the types that this account is associated with.
            violations = violations.Where(m => vioTypeId.Contains(m.Category.Type.CommonViolationTypeId));

            ///searching 
            if (!string.IsNullOrEmpty(model.Searchstring))
            {
                violations = violations.Where(s => s.Name.ToLower().Contains(model.Searchstring.ToLower())
                                                || s.CustomName.ToLower().Contains(model.Searchstring.ToLower()));
            }
            if (Convert.ToString(model.CategoryId) != "00000000-0000-0000-0000-000000000000")
            {
                violations = violations.Where(s => s.CategoryId.Equals(model.CategoryId));
            }
            if (Convert.ToString(model.TypeId) != "00000000-0000-0000-0000-000000000000")
            {
                violations = violations.Where(s => s.Category.TypeId.Equals(model.TypeId));
            }
            if (model.Actions != 0)
            {
                violations = violations.Where(s => s.CustomActions.HasFlag(model.Actions)
                || s.Actions.HasFlag(model.Actions));
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
                case "HelpUrl":
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.HelpUrl).ThenBy(s => s.CustomHelpUrl);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.HelpUrl).ThenBy(s => s.CustomHelpUrl);
                    }
                    break;
                case "Actions":
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.Actions).ThenBy(s => s.CustomActions);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.Actions).ThenBy(s => s.CustomActions);
                    }
                    break;
                default:
                    if (model.SortDirection == "DESC")
                    {
                        violations = violations.OrderByDescending(s => s.Name).ThenBy(s => s.CustomName);
                    }
                    else
                    {
                        violations = violations.OrderBy(s => s.Name).ThenBy(s => s.CustomName);
                    }
                    break;
            }

            var totalQueryCount = await violations.CountAsync();

            model.Violation = Mapper.Map<List<AccountViolationListItem>>(await violations.Skip(offset).Take(model.PageSize).ToListAsync());

            model.Paging.CurrentPage = currentPageNum;
            model.Paging.ItemsPerPage = model.PageSize;
            model.Paging.TotalItems = totalQueryCount;

            //TODO: this should be cached and retrieved from a service
            var Types = await _accountCtx.ViolationTypes.Where(m => m.AccountId.Equals(CommonAccount.Id) && vioTypeId.Contains(m.CommonViolationTypeId)).Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString(),
            })
            .ToListAsync();

            model.Types.AddRange(Types);

            //TODO: this should be cached and retrieved from a service
            var Categories = await _accountCtx.ViolationCategorys.Include(m => m.Type).Where(m => m.AccountId.Equals(CommonAccount.Id) && vioTypeId.Contains(m.Type.CommonViolationTypeId)).Select(m => new SelectListItem
            {
                Text = m.Name + " (" + m.Type.Name + " )",
                Value = m.Id.ToString(),

            }).ToListAsync();
            model.Categories.AddRange(Categories);

            var enulist = Enum.GetValues(typeof(ViolationActions)).Cast<ViolationActions>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
            model.ActionsList.AddRange(enulist);

            return View(model);
        }

        public async Task<IActionResult> EditViolation(Guid Id)
        {
            var model = new AccountViolationViewModel();

            var violationDetails = await _accountCtx.Violations.Include(x => x.Questions).ThenInclude(x => x.CreateUser)
                .SingleOrDefaultAsync(violation => violation.Id == Id);
            model = Mapper.Map<AccountViolationViewModel>(violationDetails);

            return View(model);

        }

        /// <summary>
        /// call on create violation button
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //  [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> EditViolation(AccountViolationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var accountviolation = new Violation()
                {
                    CustomName = model.CustomName?.Trim(),
                    CustomDescription = model.CustomDescription?.Trim(),
                    CustomHelpUrl = model.CustomHelpUrl,
                    CustomActions = model.CustomActions,
                    CustomRequiredFields = model.CustomRequiredFields,
                    Code = model.Code,
                    Fee = model.Fee,
                    ReminderMessage = model.ReminderMessage,
                    ReminderMinutes = model.ReminderMinutes,
                    Id = model.Id
                };


                if (!string.IsNullOrWhiteSpace(model.CustomName))
                {
                    var NameAlreadyExists = await _accountCtx.Violations.AnyAsync(
                       q => q.CustomName.ToLower() == model.CustomName.Trim().ToLower()
                       && q.Id != model.Id
                       );
                    if (NameAlreadyExists)
                    {
                        // This isn't a security risk because we've verified the Name already exists
                        ModelState.AddModelError(string.Empty, "Custom Name already exists.");
                        return View(model);

                    }
                }
                if (!string.IsNullOrWhiteSpace(model.Code))
                {
                    var CodeAlreadyExists = await _accountCtx.Violations.AnyAsync(
                        q => q.Code == model.Code && q.Id != model.Id);

                    if (CodeAlreadyExists)
                    {
                        // This isn't a security risk because we've verified the Name already exists
                        ModelState.AddModelError(string.Empty, "Code already exists.");
                        return View(model);

                    }
                }

                var result = await _violationSvc.UpdateUserAccountViolation(accountviolation, User.GetLoggedInUserId().Value, _accountCtx);

                //Purge common accounts cache
                await _cache.RemoveAsync(WebCacheKey.Violations);

                return RedirectToAction("Violations");


            }

            //TODO: this should be cached and retrieved from a service
            var Types = await _accountCtx.ViolationTypes.Where(m => m.AccountId.Equals(CommonAccount.Id)).Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString(),
            })
            .ToListAsync();

            model.Types.AddRange(Types);

            //TODO: this should be cached and retrieved from a service
            var Categories = await _accountCtx.ViolationCategorys.Include(m => m.Type).Where(m => m.AccountId.Equals(CommonAccount.Id)).Select(m => new SelectListItem
            {
                Text = m.Name + " (" + m.Type.Name + " )",
                Value = m.Id.ToString(),

            }).ToListAsync();

            model.Categories.AddRange(Categories);

            return View(model);
        }



        /// <summary>
        /// Save Violation Question
        /// </summary>
        /// <param name="violationId"></param>
        /// <param name="question"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveQuestion(Guid violationId, Guid questionId, string question, bool isPublic, ViolationQuestionType type, string choices)
        {
            var result = new ServiceResponse<ViolationQuestion>() { Success = true };
            try
            {
                result = await _violationSvc.SaveQuestion(CommonAccount.Id, violationId, questionId, question, isPublic, type, choices, User.GetLoggedInUserId().Value);

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return Json(result);
        }

        /// <summary>
        /// Get Violation Question List
        /// </summary>
        /// <param name="citationId"></param>
        /// <returns></returns>
        public async Task<IActionResult> GetQuestionByViolationId(Guid violationId)
        {
            var questionList = new List<ViolationQuestionListItem>();
            try
            {
                questionList = await _accountCtx.ViolationQuestions.Where(m => m.ViolationId == violationId).Select(m => new ViolationQuestionListItem
                {
                    Question = m.Question,
                    QuestionID = m.Id,
                    CreatedById = m.CreateUserId,
                    CreatedBy = string.IsNullOrWhiteSpace(m.CreateUser.FullName) ? m.CreateUser.Email.ToString() : m.CreateUser.FullName.ToString(),
                    CreatedDate = m.CreateUtc,
                    Order = m.Order,
                    EnableEdit = (m.CreateUserId == User.GetLoggedInUserId().Value)
                }).OrderBy(s => s.Order).ToListAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(questionList);
        }

        [HttpPost]
        public async Task<IActionResult> EditQuestion(Guid questionID)
        {
            var result = new ViolationQuestionListItem();
            if (questionID != Guid.Empty)
            {
                try
                {
                    var questionDetails = await _accountCtx.ViolationQuestions.SingleOrDefaultAsync(a => a.Id == questionID);
                    if (questionDetails.CreateUserId == User.GetLoggedInUserId().Value)
                    {
                        result = new ViolationQuestionListItem
                        {
                            Question = questionDetails.Question,
                            QuestionID = questionDetails.Id,
                            IsRequired = questionDetails.IsRequired,
                            Choices = questionDetails.Choices,
                            Type = Convert.ToString((ViolationQuestionType)questionDetails.Type)
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveQuestionOrder(List<OrderList> Orderlist)
        {
            var result = new ServiceResponse<ViolationQuestion>() { Success = true };
            try
            {
                if (Orderlist.Count > 0)
                {
                    foreach (var question in Orderlist)
                    {

                        var QuestionDetails = await _accountCtx.ViolationQuestions.SingleOrDefaultAsync(a => a.Id == question.questionID);
                        if (QuestionDetails != null)
                        {
                            //  CitationDetails.UpdateUserId = createdById;
                            QuestionDetails.UpdateUtc = DateTime.Now;
                            QuestionDetails.Order = question.order;
                            _accountCtx.ViolationQuestions.Update(QuestionDetails);
                        }
                    }
                    await _accountCtx.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }


            return Json(result);
        }



        public async Task<IActionResult> DeleteQuestion(Guid questionID)
        {
            if (questionID != Guid.Empty)
            {
                try
                {

                    var questionDetails = await _accountCtx.ViolationQuestions.SingleOrDefaultAsync(a => a.Id == questionID);
                    if (questionDetails.CreateUserId == User.GetLoggedInUserId().Value)
                    {
                        _accountCtx.Remove(questionDetails);
                        await _accountCtx.SaveChangesAsync();
                        return Json("1");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return Json("");
        }


        public IActionResult Cancel()
        {
            return RedirectToAction("Violations", "AccountSettings");
        }

        #endregion

        #region Account Setting

        public async Task<IActionResult> Info()
        {
            AccountSettingModel model = new AccountSettingModel();
            var accountExists = await _commonContext.CommonAccounts.Where(x => x.Id == CommonAccount.Id && x.OwnerUserId == User.GetLoggedInUserId().Value).SingleOrDefaultAsync();
            model = Mapper.Map<AccountSettingModel>(accountExists);
            if (model.AttachmentId != null)
            {
                var attach = await _accountCtx.Attachments.Include(x => x.Citations).SingleOrDefaultAsync(x => x.Id == model.AttachmentId);
                if (attach != null)
                {
                    //Read the file from AWS Bucket
                    var cityAppFile = await _fileService.ReadFile(attach.Key, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);
                    if (cityAppFile.FileBytes != null)
                    {
                        cityAppFile.FileStream.Position = 0;

                        // Convert byte[] to Base64 String
                        model.ImageName = Convert.ToBase64String(cityAppFile.FileBytes);
                    }
                }
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Info(AccountSettingModel model)
        {
            ModelState.Remove("AttachmentId");
            if (ModelState.IsValid)
            {
                var accountExists = await _commonContext.CommonAccounts.AsNoTracking().Where(x => x.Name.ToLower() == model.Name.ToLower() && x.OwnerUserId != model.Id).AnyAsync();
                if (accountExists)
                {
                    ModelState.AddModelError(nameof(model.Name), "Account name already exists");
                    return View(model);
                }

                //Get current file extension
                if (model.files != null)
                {
                    if (model.AttachmentId != null)
                    {
                        var attach = await _accountCtx.Attachments.Include(x => x.Citations).SingleOrDefaultAsync(x => x.Id == model.AttachmentId);
                        if (attach != null)
                        {
                            //Delete the file from AWS Bucket and Database
                            var cityAppFile = await _fileService.DeleteFile(attach.Key, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);

                            _accountCtx.Attachments.Remove(attach);
                            _accountCtx.Entry(attach).State = EntityState.Deleted;
                            await _accountCtx.SaveChangesAsync();
                            model.AttachmentId = null;
                        }
                    }


                    List<string> Types = _appSettings.ImageTypes.Split(',').ToList();
                    string ext = System.IO.Path.GetExtension(model.files.FileName).ToLower();
                    if (Types.Contains(ext))
                    {
                        if (model.files.Length > _appSettings.ImageSize)
                        {
                            //Convert bytes into MB
                            var size = (_appSettings.ImageSize / 1024f) / 1024f;
                            ModelState.AddModelError(string.Empty, $"File size  not be greater than {size} MB.");
                            return View(model);
                        }


                        Guid fileGuid = Guid.NewGuid();
                        using (var fileStream = model.files.OpenReadStream())
                        using (var ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            //convert image into Bytes
                            var fileByte1s = ms.ToArray();

                            var fileNameInFolder = $"accounts/{CommonAccount.Number}/profile/{fileGuid + ext}";

                            //Upload the file on Amazon S3 Bucket
                            var result = await _fileService.UploadFile(fileByte1s, fileNameInFolder, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, true);

                            // put dude in the database
                            Attachment attach = new Attachment()
                            {
                                AccountId = CommonAccount.Id,
                                ContentLength = model.files.Length,
                                MimeType = model.files.ContentType,
                                FileName = model.files.FileName,
                                Key = fileNameInFolder,
                                AttachmentType = model.files.FileName.GetAttachmentType(),
                                CreateUserId = User.GetLoggedInUserId().Value,
                                UpdateUserId = User.GetLoggedInUserId().Value
                            };
                            _accountCtx.Attachments.Add(attach);
                            await _accountCtx.SaveChangesAsync();
                            model.AttachmentId = attach.Id;

                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Invalid File Formate {model.files.FileName}.");
                        return View(model);

                    }
                }

                var commonAccount = await _commonContext.CommonAccounts.Include(x => x.Partition).Where(x => x.OwnerUserId == model.Id && x.Name.ToLower() == model.Name.ToLower()).SingleOrDefaultAsync();
                if (commonAccount != null)
                {
                    commonAccount.Name = model.Name;
                    commonAccount.AttachmentId = model.AttachmentId;
                    commonAccount.ContactNumber = model.ContactNumber;
                    commonAccount.CityName = model.City;
                    commonAccount.State = model.State;
                    commonAccount.Zip = model.Zip;
                    commonAccount.Address1 = model.Address1;
                    commonAccount.Address2 = model.Address2;
                    commonAccount.ContactEmail = model.ContactEmail;
                    commonAccount.AllowPublicRegistration = model.AllowPublicRegistration;
                    commonAccount.UpdateUserId = User.GetLoggedInUserId().Value;
                    commonAccount.UpdateUtc = DateTime.Now;

                    var result = await _commonAccountSvc.UpdateSettingAccount(commonAccount, User.GetLoggedInUserId().Value);

                    //Purge common accounts cache
                    await _cache.RemoveAsync(WebCacheKey.CommonAccounts);

                    await PurgeLoggedInUser();


                    if (model.AttachmentId != null)
                    {
                        var attach = await _accountCtx.Attachments.Include(x => x.Citations).SingleOrDefaultAsync(x => x.Id == model.AttachmentId);
                        if (attach != null)
                        {
                            //Read the file from AWS Bucket
                            var cityAppFile = await _fileService.ReadFile(attach.Key, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);
                            if (cityAppFile.FileBytes != null)
                            {
                                // Convert byte[] to Base64 String
                                model.ImageName = Convert.ToBase64String(cityAppFile.FileBytes);
                            }
                        }
                    }
                }

            }
            ViewBag.SucessMessage = "Saved!";
            return View(model);
        }

        #endregion


        private async Task PopulateDropDownVendors(AccountUserViewModel model)
        {
            var vendors = await _accountCtx.Vendors.Where(x => x.AccountId == CommonAccount.Id).Select(m => new SelectListItem
            {
                Text = m.Name,
                Value = m.Id.ToString(),
            }).ToListAsync();

            model.Vendors.AddRange(vendors);


            var permissionslist = Enum.GetValues(typeof(SystemPermissions)).Cast<SystemPermissions>().Where(x => x != SystemPermissions.Administrator).Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            model.SystemPermissions.AddRange(permissionslist);
        }

        public async Task<IActionResult> SendPushNotification()
        {

            var result = new ServiceResponse<UserDeviceInfo>() { Success = true };
            List<UserDeviceInfo> lstUserDevice = new List<UserDeviceInfo>();
            lstUserDevice.Add(new UserDeviceInfo() { DeviceToken = "77df959ac96c11b305ec7dc9f69bea5caf3134fddf9661407b142e11a692a156" });

            await _PushSvc.APNS_Send(lstUserDevice, "");

            return Json(result);
        }
    }
}
