using AutoMapper;
using CityApp.Api.Models;
using CityApp.Common.Models;
using CityApp.Data.Models;


namespace CityApp.Api.MappingProfiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<CommonUserAccount, CommonUserAccountModel>()
                 .ForMember(d => d.Name, o => o.MapFrom(s => s.Account.Name))
                 .ForMember(d => d.Features, o => o.MapFrom(s => s.Account.Features));

            CreateMap<CommonAccount, CachedAccount>()
                .ForMember(d => d.ViolationTypes, o => o.Ignore());

            CreateMap<CommonAccount, AccountViewModel>()
            .ForMember(d => d.Latitude, o => o.Ignore())
            .ForMember(d => d.Longitude, o => o.Ignore())
            .ForMember(d => d.AccountId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.AccountNumber, o => o.MapFrom(s => s.Number))
            .ForMember(d => d.ExpirationUtc, o => o.Ignore())
            .ForMember(d => d.Disabled, o => o.MapFrom(s => s.Archived))
            .ForMember(d => d.Distance, o => o.Ignore());

            CreateMap<CommonAccountSettings, CachedAccountSettings>();

            CreateMap<CommonUser, LoggedInUser>();

            CreateMap<CommonUser, UserProfileModel>()
                .ForMember(d => d.ProfileImageUrl, o => o.Ignore());
        }

    }
}
