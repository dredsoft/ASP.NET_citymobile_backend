using AutoMapper;
using CityApp.Areas.Admin.Models;
using CityApp.Common.Models;
using CityApp.Data.Models;
using CityApp.Web.Areas.Admin.Models.Partitions;
using CityApp.Web.Models;
using CityApp.Web.Models.Accounts;

namespace CityApp.Web.MappingProfiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<CommonAccount, CityApp.Web.Models.AccountListItem>();
            CreateMap<CommonAccount, CityApp.Areas.Admin.Models.AccountListItem>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.OwnerUser.FullName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.OwnerUser.Email));
            CreateMap<CommonAccount, CachedAccount>()
                .ForMember(d => d.ViolationTypes, o => o.Ignore())
                ;
            CreateMap<CommonAccountSettings, CachedAccountSettings>();
            CreateMap<CommonUser, UserListItem>();
            CreateMap<Partition, PartitionListItem>();

            CreateMap<CommonUserAccount, Models.CommonUserAccountModel>();

            CreateMap<Event, AccountEvent>();

            CreateMap<CachedAccount, GlobalViewDataModel>()
                .ForMember(d => d.AccountNumber, o => o.MapFrom(s => s.Number))
                .ForMember(d => d.AccountName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Permissions, o => o.Ignore())
                .ForMember(d => d.TimeZone, o => o.MapFrom(s => s.CityTimeZone))
                .ForMember(d => d.OwnerUserId, o => o.MapFrom(s => s.OwnerUserId))
                .ForMember(d => d.UserOwnsAnAccount, o => o.Ignore());

            CreateMap<Violation, AccViolationListItem>().ForMember(d => d.TypeName, o => o.Ignore());

            CreateMap<CommonAccount, AccountSettingModel>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.OwnerUserId))
                .ForMember(d => d.City, o => o.MapFrom(s => s.CityName))
                .ForMember(d => d.files, o => o.Ignore())
                .ForMember(d => d.ImageName, o => o.Ignore())
                .ForMember(d => d.OwnerId, o => o.Ignore())
                .ForMember(d => d.Cities, o => o.Ignore())
                .ForMember(d => d.Partitions, o => o.Ignore())
                .ForMember(d => d.Users, o => o.Ignore())
                .ForMember(d => d.Buckets, o => o.Ignore())
                .ForMember(d => d.AccountNumber, o => o.Ignore())
                .ForMember(d => d.AccViolationType, o => o.Ignore())
                .ForMember(d => d.CitationWorkflowItems, o => o.Ignore());
        }

    }
}
