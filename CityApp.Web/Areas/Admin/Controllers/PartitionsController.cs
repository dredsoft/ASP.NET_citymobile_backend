using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CityApp.Data;
using CityApp.Common.Caching;
using Microsoft.Extensions.Options;
using CityApp.Common.Models;
using Serilog;
using CityApp.Services;
using CityApp.Web.Areas.Admin.Models.Partitions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using CityApp.Web.Constants;
using CityApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using CityApp.Common.Extensions;
using CityApp.Common.Utilities;

namespace CityApp.Web.Areas.Admin.Controllers
{
    public class PartitionsController : BaseAdminController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<UsersController>();
        private readonly CommonAccountService _commonAccountSvc;
        private const int DefaultPageSize = 2;
        public PartitionsController(CommonContext commonContext, RedisCache redisCache, IServiceProvider serviceProvider, IOptions<AppSettings> appSettings, CommonAccountService commonAccountSvc)
            : base(commonContext, redisCache, serviceProvider, appSettings)
        {
            _commonAccountSvc = commonAccountSvc;
        }
        public async Task<IActionResult> Index(PartitionListViewModel models)
        {

            var currentPageNum = models.Page;
            var offset = (models.PageSize * currentPageNum) - models.PageSize;

            var partition = CommonContext.Partitions.AsQueryable();


            switch (models.SortOrder)
            {
                case "Name":
                    if (models.SortDirection == "DESC")
                        partition = partition.OrderByDescending(x => x.Name);
                    else
                        partition = partition.OrderBy(x => x.Name);
                    break;
                case "Occupants":
                    if (models.SortDirection == "DESC")
                        partition = partition.OrderByDescending(x => x.Occupancy);
                    else
                        partition = partition.OrderBy(x => x.Occupancy);
                    break;

                default:
                    partition = partition.OrderByDescending(x => x.Name);
                    break;

            }

            models.Partition = Mapper.Map<List<PartitionListItem>>(await partition.Skip(offset).Take(models.PageSize).ToListAsync());
            models.Paging.CurrentPage = currentPageNum;
            models.Paging.ItemsPerPage = models.PageSize;
            models.Paging.TotalItems = await partition.CountAsync();

            return View(models);
        }

        [Authorize(Roles = SystemPermission.Administrator)]
        public IActionResult Create()
        {
            var model = new PartitionViewModel();
            return View(model);

        }

        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Create(PartitionViewModel model)
        {

            if (ModelState.IsValid)
            {
                var nameAlreadyExists = CommonContext.Partitions.Any(x => x.Name.ToLower() == model.Name.ToLower());
                if (nameAlreadyExists)
                {
                    // duplicate partition name
                    ModelState.AddModelError(string.Empty, "Name already exsits.");
                    return View(model);
                }

                var partition = new Partition();
                partition.Name = model.Name;
                partition.ConnectionString = Cryptography.Encrypt(model.ConnectionString);
                partition.CreateUserId = User.GetLoggedInUserId().Value;
                partition.UpdateUserId = partition.CreateUserId = User.GetLoggedInUserId().Value;


                //put dude in the database
                using (var tx = CommonContext.Database.BeginTransaction())
                {
                    CommonContext.Partitions.Add(partition);
                    await CommonContext.SaveChangesAsync();
                    tx.Commit();
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var model = new PartitionViewModel();
            var partition = await CommonContext.Partitions.Select(x => x).Where(x => x.Id == Id).SingleOrDefaultAsync();
            model.Id = Id;
            model.Name = partition.Name;
            model.ConnectionString = Cryptography.Decrypt(partition.ConnectionString);
            model.Disabled = partition.Disabled;
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = SystemPermission.Administrator)]
        public async Task<IActionResult> Edit(PartitionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var nameAlreadyExists = CommonContext.Partitions.Any(x => x.Name.ToLower() == model.Name.ToLower() && x.Id != model.Id);
                if (nameAlreadyExists)
                {
                    // duplicate partition name
                    ModelState.AddModelError(string.Empty, "Name already exsits.");
                    return View(model);
                }

                //Get detail from database
                var partition = await CommonContext.Partitions.SingleAsync(m => m.Id == model.Id);
                partition.Name = model.Name;
                partition.ConnectionString = Cryptography.Encrypt(model.ConnectionString);
                partition.Disabled = model.Disabled;
                partition.CreateUserId = User.GetLoggedInUserId().Value;
                partition.UpdateUserId = partition.CreateUserId = User.GetLoggedInUserId().Value;

                //put dude in the database
                using (var tx = CommonContext.Database.BeginTransaction())
                {
                    CommonContext.Partitions.Update(partition);
                    await CommonContext.SaveChangesAsync();
                    tx.Commit();
                }
                return RedirectToAction("Index");

            }


            return View(model);
        }

    }
}