using AutoMapper;
using CityApp.Areas.Admin.Models;
using CityApp.Common.Models;
using CityApp.Data.Models;
using CityApp.Web.Models;
using CityApp.Web.Models.AccountSettings;
using CityApp.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.MappingProfiles
{
    public class ViolationProfile : Profile
    {
        public ViolationProfile()
        {
            CreateMap<CommonViolationType, ViolationTypeListItem>();
            CreateMap<CommonViolation, ViolationListItem>().ForMember(d => d.TypeName, o => o.Ignore());
            CreateMap<CommonViolationCategory, ViolationCategoryListItem>();

            CreateMap<CommonViolationType, ViolationType>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CommonViolationTypeId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CustomName, o => o.Ignore())
                .ForMember(d => d.CustomDescription, o => o.Ignore())
                .ForMember(d => d.AccountId, o => o.Ignore())
                .ForMember(d => d.Account, o => o.Ignore());


            CreateMap<CommonViolationCategory, ViolationCategory>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.TypeId, o => o.Ignore())
                .ForMember(d => d.Type, o => o.Ignore())
                .ForMember(d => d.CommonCategoryId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CustomName, o => o.Ignore())
                .ForMember(d => d.CustomDescription, o => o.Ignore())
                .ForMember(d => d.AccountId, o => o.Ignore())
                .ForMember(d => d.Account, o => o.Ignore());


            CreateMap<CommonViolation, Violation>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CategoryId, o => o.Ignore())
                .ForMember(d => d.Category, o => o.Ignore())
                .ForMember(d => d.CommonViolationId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.CustomName, o => o.Ignore())
                .ForMember(d => d.CustomDescription, o => o.Ignore())
                .ForMember(d => d.CustomHelpUrl, o => o.Ignore())
                .ForMember(d => d.CustomActions, o => o.Ignore())
                .ForMember(d => d.CustomRequiredFields, o => o.Ignore())
                //.ForMember(d => d.CustomWarningQuizUrl, o => o.Ignore())
                .ForMember(d => d.Code, o => o.Ignore())
                .ForMember(d => d.AccountId, o => o.Ignore())
                .ForMember(d => d.Account, o => o.Ignore())
                .ForMember(d => d.Fee, o => o.Ignore())
                .ForMember(d => d.Questions, o => o.Ignore())
                .ForMember(d => d.EvidencePackageDelivered, o => o.Ignore());



            CreateMap<Violation, AccountViolationListItem>().ForMember(d => d.TypeName, o => o.Ignore());

            CreateMap<Violation, CachedAccountViolations>()
                .ForMember(d => d.ViolationId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.ViolationCategoryId, o => o.MapFrom(s => s.CategoryId))
                .ForMember(d => d.ViolationTypeId, o => o.MapFrom(s => s.Category.TypeId))
                .ForMember(d => d.ViolationName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.ViolationCategoryName, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.ViolationTypeName, o => o.MapFrom(s => s.Category.Type.Name));

            CreateMap<ViolationType, ViolationTypeModel>();


            CreateMap<ViolationQuestion, ViolationQuestionListItem>()
            .ForMember(d => d.QuestionID, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreateUser.DisplayName))
            .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreateUtc))
            .ForMember(d => d.CreatedById, o => o.MapFrom(s => s.CreateUserId))
            .ForMember(d => d.EnableEdit, o => o.Ignore())
            .ForMember(d => d.CreatedHumanizerDate, o => o.Ignore());

            CreateMap<Violation, AccountViolationViewModel>()
                .ForMember(d => d.Types, o => o.Ignore())
                .ForMember(d => d.Categories, o => o.Ignore())
                .ForMember(d => d.TypeName, o => o.Ignore())
                .ForMember(d => d.Question, o => o.Ignore())
                .ForMember(d => d.Fields, o => o.Ignore())
                 .ForMember(d => d.QuestionId, o => o.Ignore())
                 .ForMember(d => d.IsRequired, o => o.Ignore())
                .ForMember(d => d.ViolationQuestion, o => o.MapFrom(s => s.Questions));


            CreateMap<AdminAccountViolationViewModel, Violation>()
                .ForMember(m => m.Id, o => o.Ignore())
                .ForMember(m => m.Account, o => o.Ignore())
                .ForMember(m=> m.CustomName, o => o.Ignore())
                .ForMember(m => m.CommonViolationId, o => o.Ignore())
                .ForMember(m => m.Category, o => o.Ignore())
                .ForMember(m => m.CustomDescription, o => o.Ignore())
                //.ForMember(m => m.CustomWarningQuizUrl, o => o.Ignore())
                .ForMember(m => m.CustomHelpUrl, o => o.Ignore())
                .ForMember(m => m.Disabled, o => o.Ignore())
                .ForMember(m => m.CustomActions, o => o.Ignore())
                .ForMember(m => m.EvidencePackageDelivered, o => o.Ignore())
                .ForMember(m => m.CustomRequiredFields, o => o.Ignore())
                .ForMember(m => m.Questions, o => o.Ignore())
                .ForMember(m => m.CreateUserId, o => o.Ignore())
                .ForMember(m => m.CreateUtc, o => o.Ignore())
                .ForMember(m => m.UpdateUserId, o => o.Ignore())
                .ForMember(m => m.UpdateUtc, o => o.Ignore())
                .ForMember(m => m.Timestamp, o => o.Ignore());

            CreateMap<Violation, AdminAccountViolationViewModel>()
                .ForMember(m => m.Categories, o => o.Ignore());


            CreateMap<AdminAccountCategoryViewModel, ViolationCategory>()
                .ForMember(m => m.Id, o => o.Ignore())
                .ForMember(m => m.Account, o => o.Ignore())
                .ForMember(m => m.CommonCategoryId, o => o.Ignore())
                .ForMember(m => m.Type, o => o.Ignore())
                .ForMember(m => m.CustomName, o => o.Ignore())
                .ForMember(m => m.CustomDescription, o => o.Ignore())
                .ForMember(m => m.Disabled, o => o.Ignore())
                .ForMember(m => m.CreateUserId, o => o.Ignore())
                .ForMember(m => m.CreateUtc, o => o.Ignore())
                .ForMember(m => m.UpdateUserId, o => o.Ignore())
                .ForMember(m => m.UpdateUtc, o => o.Ignore())
                .ForMember(m => m.Timestamp, o => o.Ignore());

            CreateMap<ViolationCategory, AdminAccountCategoryViewModel>()
                .ForMember(m => m.Types, o => o.Ignore());
        }
    }
}
