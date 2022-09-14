using AutoMapper;
using CityApp.Data.Models;
using CityApp.Web.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.MappingProfiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<City, CityListItem>();
            CreateMap<City, CityModel>();
        }
    }
}
