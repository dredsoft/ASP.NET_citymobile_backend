using AutoMapper;
using CityApp.Api.Models;
using CityApp.Common.Models;
using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.MappingProfile
{
    public class ViolationProfile : Profile
    {

        public ViolationProfile()
        {
            CreateMap<Violation, ViolationsListModel>()
                .ForMember(d => d.TypeName, o => o.MapFrom(s => s.Category.Type.Name))
                 .ForMember(d => d.ViolationQuestion, o => o.MapFrom(s => s.Questions));

            CreateMap<ViolationType, ViolationTypeModel>();

            CreateMap<ViolationQuestion, ViolationQuestionsListItem>()
           .ForMember(d => d.QuestionID, o => o.MapFrom(s => s.Id))
           .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreateUser.DisplayName))
           .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreateUtc))
           .ForMember(d => d.CreatedById, o => o.MapFrom(s => s.CreateUserId))
           .ForMember(d => d.EnableEdit, o => o.Ignore())
           .ForMember(d => d.CreatedHumanizerDate, o => o.Ignore());
        }
    }
}
