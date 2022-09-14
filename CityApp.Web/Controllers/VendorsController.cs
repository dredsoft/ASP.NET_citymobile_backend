using AutoMapper;
using CityApp.Common.Caching;
using CityApp.Common.Extensions;
using CityApp.Common.Models;
using CityApp.Data;
using CityApp.Data.Models;
using CityApp.Services;
using CityApp.Web.Models.Vendors;
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
    public class VendorsController : AccountBaseController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<VendorsController>();

        private readonly CommonUserService _commonUserSvc;
        private AccountContext _accountCtx;

        public VendorsController(CommonContext commonContext, IServiceProvider serviceProvider, RedisCache redisCache, IOptions<AppSettings> appSettings, AccountContext accountCtx, CommonUserService commonUserSvc)
            : base(commonContext, serviceProvider, redisCache, appSettings)
        {
            _commonUserSvc = commonUserSvc;
            _accountCtx = accountCtx;
        }

        #region Vendor

        public async Task<IActionResult> Index(VendorListViewModel model)
        {
            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            //This is an example of how to query for entities in the account context.  Most entites that live in the accountcontext will have a foreign key to Account.
            //It's important that we filter by account FIRST before applying any other filters when building quries. 
            var vendors = _accountCtx.Vendors.ForAccount(CommonAccount.Id).AsQueryable();

            switch (model.SortOrder)
            {
                case "Name":
                    if (model.SortDirection == "DESC")
                        vendors = vendors.OrderByDescending(x => x.Name);
                    else
                        vendors = vendors.OrderBy(x => x.Name);
                    break;
                case "Email":
                    if (model.SortDirection == "DESC")
                        vendors = vendors.OrderByDescending(x => x.Email);
                    else
                        vendors = vendors.OrderBy(x => x.Email);
                    break;

                default:
                    vendors = vendors.OrderByDescending(x => x.Name);
                    break;
            }


            var totalQueryCount = await vendors.CountAsync();

            model.VendorList = Mapper.Map<List<VendorListItem>>(vendors);           

            return View(model);
        }

        public  IActionResult Create()
        {         

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create(VendorsViewModel model)
        {

            if (ModelState.IsValid)
            {
                var nameAlreadyExists = await _accountCtx.Vendors.AnyAsync(q => q.AccountId == CommonAccount.Id && q.Name == model.Name);
                if (nameAlreadyExists)
                {
                    // Name must be unique.  No Duplicates.
                    ModelState.AddModelError(nameof(model.Name), "Name already exists.");
                    return View(model);
                }               

                var vendor = new Vendor();
                vendor.AccountId = CommonAccount.Id;
                vendor.Email = model.Email;
                vendor.Name = model.Name;
                vendor.MobilePhone = model.MobilePhone;
                vendor.OfficePhone = model.OfficePhone;
                vendor.Address1 = model.Address1;
                vendor.Address2 = model.Address2;
                vendor.City = model.City;
                vendor.State = model.State;
                vendor.Zip = model.Zip;
                vendor.Disabled = model.Disabled;
                vendor.Actions= model.Actions;
                vendor.CreateUserId = LoggedInUser.Id;
                vendor.UpdateUserId = LoggedInUser.Id;


                //put dude in the database
                using (var tx = _accountCtx.Database.BeginTransaction())
                {
                    _accountCtx.Vendors.Add(vendor);
                    await _accountCtx.SaveChangesAsync();
                    tx.Commit();
                }
                return RedirectToAction("Index");
            }

            return View(model);

        }

        public async Task<IActionResult> Edit(Guid Id)
        {
            VendorsViewModel model = new VendorsViewModel();
            var vendorDetail = await _accountCtx.Vendors.Where(q => q.AccountId == CommonAccount.Id && q.Id == Id).SingleOrDefaultAsync();
            if (vendorDetail != null)
            {
                model.Id = vendorDetail.Id;
                model.Name = vendorDetail.Name;
                model.Email = vendorDetail.Email;
                model.MobilePhone = vendorDetail.MobilePhone;
                model.OfficePhone = vendorDetail.OfficePhone;
                model.Address1 = vendorDetail.Address1;
                model.Address2 = vendorDetail.Address2;
                model.City = vendorDetail.City;
                model.State = vendorDetail.State;
                model.Zip = vendorDetail.Zip;
                model.Disabled = vendorDetail.Disabled;
                model.Actions = vendorDetail.Actions;
                return View(model);
            }

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(VendorsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nameAlreadyExists = await _accountCtx.Vendors.AsNoTracking().AnyAsync(q => q.AccountId == CommonAccount.Id && q.Name == model.Name && q.Id != model.Id);
                if (nameAlreadyExists)
                {
                    // Name must be unique.  No Duplicates.
                    ModelState.AddModelError(nameof(model.Name), "Name already exists.");
                    return View(model);
                }
               
                var vendorDetail = _accountCtx.Vendors.Where(q => q.AccountId == CommonAccount.Id && q.Id == model.Id).SingleOrDefault();
                if (vendorDetail != null)
                {

                    vendorDetail.Name = model.Name;
                    vendorDetail.Email = model.Email;
                    vendorDetail.MobilePhone = model.MobilePhone;
                    vendorDetail.OfficePhone = model.OfficePhone;
                    vendorDetail.Address1 = model.Address1;
                    vendorDetail.Address2 = model.Address2;
                    vendorDetail.City = model.City;
                    vendorDetail.State = model.State;
                    vendorDetail.Zip = model.Zip;
                    vendorDetail.Disabled = model.Disabled;
                    vendorDetail.Actions = model.Actions;


                    //put dude in the database
                    using (var tx = _accountCtx.Database.BeginTransaction())
                    {
                        _accountCtx.Vendors.Update(vendorDetail);
                        await _accountCtx.SaveChangesAsync();
                        tx.Commit();
                    }
                    return RedirectToAction("Index");
                }


            }
            return View(model);
        }


        public async Task<IActionResult> Users(AccountUserVendorListViewModel model)
        {
            
            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;

            //This is an example of how to query for entities in the account context.  Most entites that live in the accountcontext will have a foreign key to Account.
            //It's important that we filter by account FIRST before applying any other filters when building quries. 
            var UserVendor = _accountCtx.AccountUserVendors.Include(m=>m.AccountUser).Where(x => x.VendorId == model.Id).AsQueryable();

            switch (model.SortOrder)
            {
                case "Name":
                    if (model.SortDirection == "DESC")
                        UserVendor = UserVendor.OrderByDescending(x => x.AccountUser.FirstName);
                    else
                        UserVendor = UserVendor.OrderBy(x => x.AccountUser.FirstName);
                    break;
                case "Email":
                    if (model.SortDirection == "DESC")
                        UserVendor = UserVendor.OrderByDescending(x => x.AccountUser.Email);
                    else
                        UserVendor = UserVendor.OrderBy(x => x.AccountUser.Email);
                    break;

                default:
                    UserVendor = UserVendor.OrderBy(x => x.AccountUser.FirstName);
                    break;
            }


            var totalQueryCount = await UserVendor.CountAsync();

            model.AccountUserVendorListItem = Mapper.Map<List<AccountUserVendorListItem>>(UserVendor);

            return View(model);

        }

        #endregion;
    }
}
