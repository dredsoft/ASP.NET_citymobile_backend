using AutoMapper;
using CityApp.Web.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.MappingProfiles
{
    public class CitationReceipt : Profile
    {
        public CitationReceipt()
        {
            CreateMap<CitationReceiptViewModel, CitationDeviceReceiptModel>()
               .ForMember(d => d.submittedUtc, o => o.MapFrom(s => s.Submitted))
               .ForMember(d => d.device, o => o.MapFrom(s => s.Device))
               .ForMember(d => d.useremail, o => o.MapFrom(s => s.Email))
               .ForMember(d => d.latitude, o => o.MapFrom(s => s.Latitude))
               .ForMember(d => d.longitude, o => o.MapFrom(s => s.Longitude))
                .ForMember(d => d.identifier, o => o.Ignore())
                .ForMember(d => d.userId, o => o.Ignore())
                .ForMember(d => d.publicKey, o => o.Ignore())
                .ForMember(d => d.files, o => o.Ignore());

        }
    }
}
