using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CityApp.Data;
using Serilog;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using CityApp.Web.Models.User;
using CityApp.Data.Models;
using System.Security.Claims;
using CityApp.Data.Enums;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using CityApp.Common.Caching;
using CityApp.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using CityApp.Services;
using CityApp.Common.Utilities;
using CityApp.Common.Extensions;
using CityApp.Common.Constants;
using System.IO;

namespace CityApp.Web.Controllers
{

    public class UserController : BaseController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<UserController>();
        private readonly CommonUserService _commonUserSvc;
        private readonly CommonAccountService _commonAccountSvc;
        private readonly MailService _mailSvc;
        private AccountContext _accountCtx;
        private readonly AppSettings _appSettings;
        private readonly FileService _fileService;

        public UserController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache redisCache, IOptions<AppSettings> appSettings, CommonUserService commonUserSvc, MailService mailSvc, CommonAccountService commonAccountSvc, AccountContext accountCtx, FileService fileService)
            : base(commonContext, serviceProvider, redisCache, appSettings)
        {
            _commonUserSvc = commonUserSvc;
            _mailSvc = mailSvc;
            _commonAccountSvc = commonAccountSvc;
            _accountCtx = accountCtx;
            _appSettings = appSettings.Value;
            _fileService = fileService;
        }

        public IActionResult Register(string email = null, string returnUrl = null)
        {
            ModelState.Clear();
            return View(new UserRegisterModel { Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(UserRegisterModel model)
        {
            if (ModelState.IsValid)
            {

                var userAlreadyExists = await CommonContext.Users.AnyAsync(q => q.Email.ToLower() == model.Email.ToLower());

                if (userAlreadyExists)
                {
                    // This isn't a security risk because we've verified the email address already
                    ModelState.AddModelError(string.Empty, "A user has already verified that email address.");
                }
                else
                {

                    var user = new CommonUser();

                    user.Email = model.Email;
                    user.SetPassword(model.Password);
                    user.Permission = IsAdmin(model.Email);

                    //put dude in the database
                    using (var tx = CommonContext.Database.BeginTransaction())
                    {
                        CommonContext.Users.Add(user);
                        await CommonContext.SaveChangesAsync();
                        tx.Commit();

                        await SignInUser(user, false);

                        return RedirectToAction("Index", "Accounts", new { Area = Area.Admin });
                    }
                }
            }

            return View(model);
        }

        private SystemPermissions IsAdmin(string email)
        {
            var whiteListEmailDomains = AppSettings.WhiteListAdmin.Split(',').Select(m => m.ToLower()).ToList();

            MailAddress address = new MailAddress(email);
            string host = address.Host; // host contains yahoo.com

            var isInWhiteList = whiteListEmailDomains.Contains(host);

            if (isInWhiteList)
            {
                return SystemPermissions.Administrator;
            }
            else
            {
                return SystemPermissions.None;
            }
        }

        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _commonUserSvc.GetUser(model.Email, model.Password);
                if (user != null)
                {
                    await SignInUser(user, false);

                    if (user.Permission == SystemPermissions.Administrator)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Login");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //return RedirectToAction("Index", "Dashboard");
            return RedirectToAction("Login", "User");
        }

        public IActionResult TermCondition()
        {
            return View();
        }


        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = new UserModel();
            var user = await CommonContext.Users.Select(x => x).Where(x => x.Id == Id).SingleOrDefaultAsync();

            model.Id = Id;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Email = user.Email;

            if (!string.IsNullOrWhiteSpace(user.ProfileImageKey))
            {
                //Read the file from AWS Bucket
                var cityAppFile = await _fileService.ReadFile(user.ProfileImageKey, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);

                if (cityAppFile.FileBytes != null)
                {
                    cityAppFile.FileStream.Position = 0;

                    // Convert byte[] to Base64 String
                    model.ImageName = Convert.ToBase64String(cityAppFile.FileBytes);
                    model.ProfileImageKey = user.ProfileImageKey;
                }

            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(UserModel model)
        {
            ModelState.Remove("ProfileImageKey");
            if (ModelState.IsValid)
            {
                var userAlreadyExists = await CommonContext.Users.AsNoTracking().AnyAsync(q => q.Email.ToLower() == model.Email.ToLower() && q.Id != model.Id);
                if (userAlreadyExists)
                {
                    // This isn't a security risk because we've verified the email address already
                    ModelState.AddModelError(string.Empty, "A user has already verified that email address.");

                    return View(model);
                }

                //Get current file extension
                if (model.files != null)
                {
                   
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

                        if (model.ProfileImageKey != null)
                        {
                            //Delete the file from AWS Bucket and Database
                            var cityAppFile = await _fileService.DeleteFile(model.ProfileImageKey, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);
                        }


                        Guid fileGuid = Guid.NewGuid();
                        using (var fileStream = model.files.OpenReadStream())
                        using (var ms = new MemoryStream())
                        {
                            fileStream.CopyTo(ms);
                            //convert image into Bytes
                            var fileByte1s = ms.ToArray();
                            //Common/User/(UserId)/Profile/(new Guid()).(ext)
                            var fileNameInFolder = $"common/User/{User.GetLoggedInUserId().Value}/profile/{fileGuid + ext}";

                            //Upload the file on Amazon S3 Bucket
                            var result = await _fileService.UploadFile(fileByte1s, fileNameInFolder, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, true);
                            model.ProfileImageKey = fileNameInFolder;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Invalid File Formate {model.files.FileName}.");
                        return View(model);

                    }
                }



                var user = await CommonContext.Users.SingleAsync(m => m.Id == model.Id);
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.ProfileImageKey = model.ProfileImageKey;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    if (!string.IsNullOrWhiteSpace(model.OldPassword))
                    {
                        if (user.CheckPassword(model.OldPassword))
                        {
                            user.SetPassword(model.Password);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid old password.");
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid old password.");
                        return View(model);
                    }
                }

                CommonContext.Users.Update(user);
                await CommonContext.SaveChangesAsync();

                //Update user information in all other accounts
                await _commonAccountSvc.SaveAccountUser(user, User.GetLoggedInUserId().Value);

                //Update user information in cache 
                await PurgeLoggedInUser();

            }

            if (!string.IsNullOrWhiteSpace(model.ProfileImageKey))
            {
                //Read the file from AWS Bucket
                var cityAppFile = await _fileService.ReadFile(model.ProfileImageKey, _appSettings.AWSAccessKeyID, _appSettings.AWSSecretKey, _appSettings.AmazonS3Bucket);

                if (cityAppFile.FileBytes != null)
                {
                    cityAppFile.FileStream.Position = 0;
                    // Convert byte[] to Base64 String
                    model.ImageName = Convert.ToBase64String(cityAppFile.FileBytes);
                }

            }

            ViewBag.SucessMessage = "Your profile has been updated";
            return View(model);
        }




        #region ForgetPassword
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPassword model)
        {

            if (ModelState.IsValid)
            {
                CommonUser user = await _commonUserSvc.CheckUser(model.Email);
                if (user != null)
                {
                    string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    var callbackUrl = Url.Action("ResetPassword", "User", new { code = token });

                    using (var tx = CommonContext.Database.BeginTransaction())
                    {
                        user.Token = token;
                        user.TokenUtc = DateTime.UtcNow;
                        CommonContext.Users.Update(user);

                        await CommonContext.SaveChangesAsync();
                        tx.Commit();

                    }

                    _logger.Error($"Sending Email email:{model.Email} callback:{callbackUrl}");
                    bool result = await _mailSvc.SendPasswordResetEmail(model.Email, callbackUrl);
                    _logger.Error($"Email Sent:");



                }
                ViewBag.SucessMessage = "Please check your email for a link to reset your password";
                return View();
            }

            return View(model);

        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword(string code)
        {
            if (code != null)
            {
                ResetPassword model = new Models.User.ResetPassword();
                var _tokenDetail = (from a in CommonContext.Users where a.Token == code select new { a }).SingleOrDefault();
                if (_tokenDetail != null)
                {
                    DateTime CurrentDate = DateTime.UtcNow;
                    DateTime tokenDate = Convert.ToDateTime(_tokenDetail.a.TokenUtc);
                    var _Diff = CurrentDate.Subtract(tokenDate).TotalHours;
                    if (_Diff <= 72)
                    {
                        model.Id = Convert.ToString(_tokenDetail.a.Id);
                        return View(model);
                    }
                    return View("LinkExpired");
                }

            }

            return View("LinkExpired");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (ModelState.IsValid)
            {

                Guid userid = new Guid(model.Id);
                var userDetail = (from a in CommonContext.Users where a.Id == userid select new { a }).SingleOrDefault();
                if (userDetail != null)
                {
                    userDetail.a.SetPassword(model.Password);
                    using (var tx = CommonContext.Database.BeginTransaction())
                    {
                        userDetail.a.Token = null;
                        CommonContext.Users.Update(userDetail.a);
                        await CommonContext.SaveChangesAsync();
                        tx.Commit();
                    }
                    await SignInUser(userDetail.a, false);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);

        }

        public IActionResult LinkExpired()
        {
            return View();
        }

        #endregion


        #region Invitation
        [AllowAnonymous]
        [HttpGet]
        public ActionResult AcceptInvite(string code)
        {
            AcceptInvite model = new AcceptInvite();
            string[] Decode = Cryptography.Decrypt(code).Split('/');
            var datetime = Decode[4];
            DateTime CurrentDate = DateTime.Now;
            DateTime tokenDate = Convert.ToDateTime(datetime);
            var _Diff = CurrentDate.Subtract(tokenDate).TotalHours;
            if (_Diff <= 24)
            {

                Guid accountId = new Guid(Decode[2]);
                Guid VendorId;
                if (!string.IsNullOrWhiteSpace(Decode[5]))
                {
                    VendorId = new Guid(Decode[5]);
                }
                else
                {
                    VendorId = new Guid("00000000-0000-0000-0000-000000000000");
                }
                var accDetail = (from a in CommonContext.CommonAccounts where a.Id == accountId select new { a.Name }).SingleOrDefault();
                if (accDetail != null)
                {
                    model.AccountName = accDetail.Name;
                    model.AccountId = accountId;
                    model.InvitedEmail = Decode[0];
                    model.Permission = Convert.ToInt16(Decode[1]);
                    model.Name = Decode[3];
                    model.VendorId = VendorId;
                    //TODO: Get system permission from code.  For now, we will default to Government. 
                    model.SystemPermission = (SystemPermissions)Enum.Parse(typeof(SystemPermissions), (Convert.ToString(Decode[6])));

                    return View(model);
                }
                return RedirectToAction("Index", "Accounts");
            }
            ViewBag.SucessMessage = "User Invitation Link is Expired.";
            return View(model);


        }

        [HttpPost]
        public async Task<ActionResult> AcceptInvite(AcceptInvite model)
        {
            //Check to see if this user exists in Users
            var existingUser = await CommonContext.Users.Where(m => m.Email == model.InvitedEmail).AnyAsync();
            if (existingUser)
            {

                //Check to see if this user already belongs to the account
                //Users can belong to multiple accounts, we need to filter by accountID and InvitedEmail
                var existingAccountUser = await CommonContext.UserAccounts.Include(x => x.User).Where(m => m.User.Email == model.InvitedEmail && m.AccountId == model.AccountId).AnyAsync();
                if (!existingAccountUser)
                {
                    //Get exists User Detail 
                    var user = CommonContext.Users.Where(m => m.Email == model.InvitedEmail).SingleOrDefault();

                    //Get Account detail using accountId                   
                    var commonAccount = await CommonContext.CommonAccounts.Where(m => m.Id == model.AccountId).SingleOrDefaultAsync();

                    var vendorDetail = await _accountCtx.Vendors.Where(x => x.Id == model.VendorId).SingleOrDefaultAsync();

                    if (commonAccount == null)
                    {
                        ViewBag.SucessMessage = "Account not found";
                        return View(model);
                    }

                    CommonUserAccount userAccount = new CommonUserAccount();
                    userAccount.CreateUserId = commonAccount.OwnerUserId.Value;
                    userAccount.AccountId = commonAccount.Id;
                    userAccount.UpdateUserId = commonAccount.OwnerUserId.Value;
                    userAccount.UserId = user.Id;
                    userAccount.Permissions = (AccountPermissions)model.Permission;

                    using (var tx = CommonContext.Database.BeginTransaction())
                    {
                        CommonContext.UserAccounts.Add(userAccount);
                        await CommonContext.SaveChangesAsync();

                        tx.Commit();
                    }

                    var accountUser = await _accountCtx.AccountUsers.Where(m => m.Id == user.Id).SingleOrDefaultAsync();

                    if (accountUser == null)
                    {
                        //Create AccountUser
                        accountUser = new AccountUser()
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                        };
                        _accountCtx.AccountUsers.Add(accountUser);
                        await _accountCtx.SaveChangesAsync();
                    }

                    if (vendorDetail != null)
                    {
                        AccountUserVendor accountUserVendor = new AccountUserVendor();
                        accountUserVendor.VendorId = vendorDetail.Id;
                        accountUserVendor.AccountUserId = accountUser.Id;

                        user.Permission = SystemPermissions.Vendor;
                        CommonContext.SaveChanges();

                        using (var tx = _accountCtx.Database.BeginTransaction())
                        {
                            _accountCtx.AccountUserVendors.Add(accountUserVendor);
                            await _accountCtx.SaveChangesAsync();
                            tx.Commit();
                        }


                    }

                    return RedirectToAction("Index", "Accounts");
                }
                else
                {
                    ViewBag.SucessMessage = " Already Account user.";
                    return View(model);
                }
            }
            else
            {
                CreatePassword pwdmodel = new Models.User.CreatePassword();
                pwdmodel.AccountId = model.AccountId;
                pwdmodel.AccountName = model.AccountName;
                pwdmodel.InvitedEmail = model.InvitedEmail;
                string[] name = model.Name.Split(' ');
                pwdmodel.FirstName = name[0];
                pwdmodel.LastName = name[1];
                pwdmodel.Permission = model.Permission;
                pwdmodel.VendorId = model.VendorId;
                pwdmodel.SystemPermission = model.SystemPermission;
                return View("CreatePassword", pwdmodel);

            }


        }

        [HttpPost]
        public async Task<ActionResult> CreatePassword(CreatePassword model)
        {

            if (ModelState.IsValid)
            {
                //Create new User and Account
                var user = new CommonUser();
                user.Email = model.InvitedEmail;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Permission = model.SystemPermission;
                user.SetPassword(model.Password);


                //Check Account
                var commonAccount = await CommonContext.CommonAccounts.Where(m => m.Id == model.AccountId).SingleOrDefaultAsync();

                var vendorDetail = await _accountCtx.Vendors.Where(x => x.Id == model.VendorId).SingleOrDefaultAsync();


                if (commonAccount == null)
                {
                    ViewBag.SucessMessage = "Account not found";
                    return View(model);
                }

                CommonUserAccount userAccount = new CommonUserAccount();
                userAccount.CreateUserId = commonAccount.OwnerUserId.Value;
                userAccount.AccountId = commonAccount.Id;
                userAccount.UpdateUserId = commonAccount.OwnerUserId.Value;
                userAccount.UserId = user.Id;
                userAccount.Permissions = (AccountPermissions)model.Permission;

                if(vendorDetail != null)
                {
                    user.Permission = SystemPermissions.Vendor;
                }

                using (var tx = CommonContext.Database.BeginTransaction())
                {
                    CommonContext.Users.Add(user);
                    await CommonContext.SaveChangesAsync();

                    CommonContext.UserAccounts.Add(userAccount);
                    await CommonContext.SaveChangesAsync();

                    tx.Commit();
                }

                //Create AccountUser
                var accountUser = new AccountUser
                {
                    Id = userAccount.UserId.Value,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };

                _accountCtx.AccountUsers.Add(accountUser);

                if (vendorDetail != null)
                {
                    AccountUserVendor accountUserVendor = new AccountUserVendor();
                    accountUserVendor.VendorId = vendorDetail.Id;
                    accountUserVendor.AccountUserId = accountUser.Id;

                    _accountCtx.AccountUserVendors.Add(accountUserVendor);
                }

                await _accountCtx.SaveChangesAsync();

                await SignInUser(user, false);

                return RedirectToAction("Index", "Accounts");


            }
            return View(model);
        }
        #endregion

        #region private helpers

        private async Task SignInUser(CommonUser user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(Common.Constants.Security.Email, user.Email)
            };

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                claims.Add(new Claim(Common.Constants.Security.FirstName, user.FirstName));
            }

            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                claims.Add(new Claim(Common.Constants.Security.LastName, user.LastName));
            }

            if (user.Permission == SystemPermissions.Administrator)
            {
                claims.Add(new Claim(ClaimTypes.Role, SystemPermissions.Administrator.ToString()));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties { IsPersistent = rememberMe };

            await HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
        }

        #endregion

    }
}
