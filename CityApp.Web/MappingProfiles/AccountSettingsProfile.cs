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
    public class AccountSettingsProfile : Profile
    {
        public AccountSettingsProfile()
        {
            CreateMap<CommonUserAccount, AccountUserListItem>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email))
                .ForMember(d => d.UserType, o => o.MapFrom(s => s.User.Permission));

            CreateMap<CommonUserAccount, AccountUserViewModel>()
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.User.FirstName))
                .ForMember(d => d.MiddleName, o => o.MapFrom(s => s.User.MiddleName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.User.LastName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email))
                .ForMember(d => d.IsVendor, o => o.Ignore())
                .ForMember(d => d.VendorId, o => o.Ignore())
                .ForMember(d => d.systemPermission, o => o.Ignore())
                .ForMember(d => d.SystemPermissions, o => o.Ignore())
            .ForMember(d => d.Vendors, o => o.Ignore());

        }
    }
}
