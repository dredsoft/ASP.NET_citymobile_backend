using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using CityApp.Common.Caching;
using CityApp.Data;
using CityApp.Common.Models;
using Microsoft.Extensions.Options;

using CityApp.Web.Areas.Admin.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using CityApp.Data.Models;
using CityApp.Common.Extensions;

namespace CityApp.Web.Areas.Admin.Controllers
{
    public class CityController : BaseAdminController
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<CityController>();


        public CityController(CommonContext commonContext, RedisCache redisCache, IServiceProvider serviceProvider, IOptions<AppSettings> appSettings)
            : base(commonContext, redisCache, serviceProvider, appSettings)
        {

        }

        public async Task<IActionResult> Index(CityListViewModel model)
        {

            var currentPageNum = model.Page;
            var offset = (model.PageSize * currentPageNum) - model.PageSize;
            //Convert list to generic IEnumerable using AsQueryable          
            var cities = CommonContext.Cities.AsQueryable();


            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                cities = cities.Where(x => x.Name.ToLower().Contains(model.Name.ToLower()));

            }
            if (!string.IsNullOrWhiteSpace(model.State))
            {
                cities = cities.Where(x => x.State.ToLower().Contains(model.State.ToLower()));
            }

            switch (model.SortOrder)
            {
                case "Name":
                    if (model.SortDirection == "DESC")
                        cities = cities.OrderByDescending(x => x.Name);
                    else
                        cities = cities.OrderBy(x => x.Name);
                    break;

                case "State":
                    if (model.SortDirection == "DESC")
                        cities = cities.OrderByDescending(x => x.State);
                    else
                        cities = cities.OrderBy(x => x.State);
                    break;

                default:
                    cities = cities.OrderByDescending(x => x.CreateUtc);
                    break;
            }

            model.Paging.TotalItems = await cities.CountAsync();

            model.CityList = Mapper.Map<List<CityListItem>>(await cities.Skip(offset).Take(model.PageSize).ToListAsync());

            model.Paging.CurrentPage = currentPageNum;
            model.Paging.ItemsPerPage = model.PageSize;

            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CityModel model)
        {
            if (ModelState.IsValid)
            {
                var cityExists = await CommonContext.Cities.Where(x => x.Name.ToLower() == model.Name.ToLower()).AnyAsync();
                if (cityExists)
                {
                    ModelState.AddModelError(nameof(model.Name), "This name already exists.");
                    return View(model);
                }

                var city = new City()
                {
                    County = model.County,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    Name = model.Name,
                    State = model.State,
                    StateCode = model.StateCode,
                    TimeZone = model.TimeZone,
                    Type = model.Type,
                    CreateUserId = User.GetLoggedInUserId().Value,
                    UpdateUserId = User.GetLoggedInUserId().Value,
                };

                using (var tx = CommonContext.Database.BeginTransaction())
                {
                    CommonContext.Cities.Add(city);
                    await CommonContext.SaveChangesAsync();
                    tx.Commit();
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }


        public async Task<IActionResult> Edit(Guid Id)
        {
            CityModel model = new CityModel();
            var city = await CommonContext.Cities.Where(x => x.Id == Id).SingleOrDefaultAsync();
            if (city != null)
            {
                model = Mapper.Map<CityModel>(city);

            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CityModel model)
        {
            if (ModelState.IsValid)
            {
                var cityExists = await CommonContext.Cities.Where(x => x.Name.ToLower() == model.Name.ToLower() && x.Id != model.Id).AnyAsync();
                if (cityExists)
                {
                    ModelState.AddModelError(nameof(model.Name), "This name already exists.");
                    return View(model);
                }


                var city = await CommonContext.Cities.SingleAsync(m => m.Id == model.Id);
                city.County = model.County;
                city.Latitude = model.Latitude;
                city.Longitude = model.Longitude;
                city.Name = model.Name;
                city.State = model.State;
                city.StateCode = model.StateCode;
                city.TimeZone = model.TimeZone;
                city.Type = model.Type;
                city.UpdateUserId = User.GetLoggedInUserId().Value;

                CommonContext.Cities.Update(city);
                await CommonContext.SaveChangesAsync();

                return RedirectToAction("Index");

            }

            return View(model);
        }

    }
}