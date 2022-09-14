using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityApp.Common.Caching;
using CityApp.Common.Models;
using CityApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using CityApp.Web.Constants;
using CityApp.Areas.Admin.Models;
using CityApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CityApp.Data.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Authentication;
using System.Net.Mail;
using CityApp.Web.Areas.Admin.Models.Users;
using CityApp.Services;
using CityApp.Areas.Admin.Models.Users;
using CityApp.Common.Extensions;
using CityApp.Services.Models;

namespace CityApp.Web.Areas.Admin.Controllers
{
    public class UsersController : BaseAdminController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<UsersController>();
        private readonly CommonAccountService _commonAccountSvc;
        private readonly MailService _mailSvc;
        private const int DefaultPageSize = 10;

        public UsersController(CommonContext commonContext, RedisCache redisCache, IServiceProvider serviceProvider, IOptions<AppSettings> appSettings, CommonAccountService commonAccountSvc, MailService mailSvc)
            : base(commonContext, redisCache, serviceProvider, appSettings)
        {
            _commonAccountSvc = commonAccountSvc;
            _mailSvc = mailSvc;
        }

        //TODO: List Pages, and most pages, should be async
        public async Task<IActionResult> Index(UserListViewModel model)
        {
            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;
            //Convert list to generic IEnumerable using AsQueryable          
            var user = CommonContext.Users.AsQueryable();


            if (!string.IsNullOrWhiteSpace(model.Permission))
            {
                user = user.Where(x => x.Permission == (SystemPermissions)Enum.Parse(typeof(SystemPermissions), model.Permission));
            }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                user = user.Where(x => x.Email.ToLower().Contains(model.Email.ToLower()));

            }
            if (!string.IsNullOrWhiteSpace(model.FirstName))
            {
                user = user.Where(x => x.FirstName.ToLower().Contains(model.FirstName.ToLower()));

            }
            if (!string.IsNullOrWhiteSpace(model.LastName))
            {
                user = user.Where(x => x.LastName.ToLower().Contains(model.LastName.ToLower()));

            }

            switch (model.SortOrder)
            {
                case "FirstName":
                    if (model.SortDirection == "DESC")
                        user = user.OrderByDescending(x => x.FirstName);
                    else
                        user = user.OrderBy(x => x.FirstName);
                    break;
                case "LastName":
                    if (model.SortDirection == "DESC")
                        user = user.OrderByDescending(x => x.LastName);
                    else
                        user = user.OrderBy(x => x.LastName);
                    break;
                case "Email":
                    if (model.SortDirection == "DESC")
                        user = user.OrderByDescending(x => x.Email);
                    else
                        user = user.OrderBy(x => x.Email);
                    break;
                case "Permission":
                    if (model.SortDirection == "DESC")
                        user = user.OrderByDescending(x => x.Permission);
                    else
                        user = user.OrderBy(x => x.Permission);
                    break;

                default:
                    user = user.OrderByDescending(x => x.FirstName);
                    break;
            }

            model.Paging.TotalItems = await user.CountAsync();

            model.UsersList = Mapper.Map<List<UserListItem>>(await user.Skip(offset).Take(model.PageSize).ToListAsync());

            var enulist = Enum.GetValues(typeof(SystemPermissions)).Cast<SystemPermissions>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            model.systemPermission.AddRange(enulist);

            model.Paging.CurrentPage = currentPageNum;
            model.Paging.ItemsPerPage = model.PageSize;

            return View(model);

        }

        [Authorize(Roles = SystemPermission.Administrator)]
        public IActionResult Create()
        {
            var model = new UserViewModel();

            PopulateDropDownUsers(model);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Create(UserViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userAlreadyExists = await CommonContext.Users.AnyAsync(q => q.Email.ToLower() == model.Email.ToLower());
                if (userAlreadyExists)
                {
                    // This isn't a security risk because we've verified the email address already
                    ModelState.AddModelError(string.Empty, "This email address already exists.");
                    PopulateDropDownUsers(model);
                    return View(model);
                }

                var user = new CommonUser();
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.SetPassword(model.Password);
                user.Permission = model.Permission;
                user.CreateUserId = User.GetLoggedInUserId().Value;
                user.UpdateUserId = user.CreateUserId = User.GetLoggedInUserId().Value;


                //put dude in the database
                using (var tx = CommonContext.Database.BeginTransaction())
                {
                    CommonContext.Users.Add(user);
                    await CommonContext.SaveChangesAsync();
                    tx.Commit();
                }
                return RedirectToAction("Index");

            }

            //, "users", new { Area = Area.Admin });
            PopulateDropDownUsers(model);
            return View(model);

        }


        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = new EditUserViewModel();
            var user = await CommonContext.Users.Select(x => x).Where(x => x.Id == Id).SingleOrDefaultAsync();
            model.Id = Id;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Permission = user.Permission;
            model.Email = user.Email;

            PopulateDropDownEditUsers(model);
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userAlreadyExists = await CommonContext.Users.AsNoTracking().AnyAsync(q => q.Email.ToLower() == model.Email.ToLower() && q.Id != model.Id);
                if (userAlreadyExists)
                {
                    // This isn't a security risk because we've verified the email address already
                    ModelState.AddModelError(string.Empty, "A user has already verified that email address.");
                    PopulateDropDownEditUsers(model);
                    return View(model);
                }

                var user = await CommonContext.Users.SingleAsync(m => m.Id == model.Id);
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Permission = model.Permission;

                if (model.Password != null)
                {
                    user.SetPassword(model.Password);
                }

                CommonContext.Users.Update(user);
                await CommonContext.SaveChangesAsync();

                //Update user information in all other accounts
                await _commonAccountSvc.SaveAccountUser(user, User.GetLoggedInUserId().Value);

                if (User.GetLoggedInUserId().Value == user.Id)
                {
                    await PurgeLoggedInUser();
                }



                return RedirectToAction("Index");
            }

            PopulateDropDownEditUsers(model);
            return View(model);
        }

        public IActionResult Cancel()
        {

            return RedirectToAction("Index", "users", new { Area = Area.Admin });

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

        #region
        private void PopulateDropDownUsers(UserViewModel model)
        {

            var enulist = Enum.GetValues(typeof(SystemPermissions)).Cast<SystemPermissions>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            model.systemPermission.AddRange(enulist);
        }

        private void PopulateDropDownEditUsers(EditUserViewModel model)
        {

            var enulist = Enum.GetValues(typeof(SystemPermissions)).Cast<SystemPermissions>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            model.systemPermission.AddRange(enulist);
        }
        #endregion




        #region private helpers

        private async Task SignInUser(CommonUser user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString())
            };

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

