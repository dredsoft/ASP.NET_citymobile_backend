using AutoMapper;
using CityApp.Api.Models;
using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.MappingProfile
{
    public class AttachmentProfile : Profile
    {
        public AttachmentProfile()
        {
            CreateMap<Attachment, AttachmentModel>()
                .ForMember(d => d.AttachmentType, o => o.MapFrom(s => s.AttachmentType))
                .ForMember(d => d.Citations, o => o.Ignore());

            CreateMap<CitationAttachment, AttachmentModel>()
                .ForMember(d => d.AttachmentType, o => o.MapFrom(s => s.Attachment.AttachmentType))
                .ForMember(d => d.Id, o => o.MapFrom(s => s.AttachmentId))
                .ForMember(d => d.FileName, o => o.MapFrom(s => s.Attachment.FileName))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Attachment.Description))
                .ForMember(d => d.MimeType, o => o.MapFrom(s => s.Attachment.MimeType))
                .ForMember(d => d.Key, o => o.MapFrom(s => s.Attachment.Key))
                .ForMember(d => d.ContentLength, o => o.MapFrom(s => s.Attachment.ContentLength))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.Attachment.Duration))
                .ForMember(d => d.DisplayDuration, o => o.MapFrom(s => s.Attachment.DisplayDuration))
                .ForMember(d => d.Citations, o => o.Ignore());

        }
    }
}
