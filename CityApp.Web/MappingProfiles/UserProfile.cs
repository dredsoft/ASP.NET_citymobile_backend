using AutoMapper;
using CityApp.Common.Models;
using CityApp.Data.Models;
using CityApp.Web.Models.Vendors;

namespace CityApp.Web.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CommonUser, LoggedInUser>();
            CreateMap<Vendor, VendorListItem>();
            CreateMap<AccountUserVendor, AccountUserVendorListItem>()
                .ForMember(d => d.Email, o => o.MapFrom(s => s.AccountUser.Email))
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.AccountUser.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.AccountUser.LastName))
                .ForMember(d => d.Id, o => o.MapFrom(s => s.AccountUser.Id));

        }
    }
}
