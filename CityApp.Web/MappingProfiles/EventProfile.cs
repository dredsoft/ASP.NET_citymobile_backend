using AutoMapper;
using CityApp.Areas.Admin.Models;
using CityApp.Data.Models;
using CityApp.Web.Models;
using CityApp.Web.Models.AccountSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.MappingProfiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventListItem>()
                .ForMember(d => d.Created, o => o.MapFrom(s => s.CreateUtc));


            CreateMap<Event, EventViewModel>()
                .ForMember(d => d.Violations, o => o.Ignore());

            CreateMap<EventBoundaryCoordinate, EventCoordinatesViewModel>().ReverseMap();

            CreateMap<EventViolationPrice, EventPricingViewModel>().ReverseMap();

        }
    }
}
