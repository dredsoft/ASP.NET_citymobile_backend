using AutoMapper;
using CityApp.Data.Extensions;
using CityApp.Data.Models;
using CityApp.Web.Areas.Admin.Models;
using CityApp.Web.Models;
using CityApp.Web.Models.Citations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.MappingProfiles
{
    public class CitationsProfile : Profile
    {
        //CitationsListItem
        public CitationsProfile()
        {
            CreateMap<Citation, CitationsListItem>()
               .ForMember(d => d.Email, o => o.MapFrom(s => s.AssignedTo.Email))
               .ForMember(d => d.FirstName, o => o.MapFrom(s => s.AssignedTo.FirstName))
               .ForMember(d => d.LastName, o => o.MapFrom(s => s.AssignedTo.LastName))
               .ForMember(d => d.Created, o => o.MapFrom(s => s.CreateUtc))
               .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
               .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
               .ForMember(d => d.Violation, o => o.MapFrom(s => s.Violation))
               .ForMember(d => d.Name, o => o.MapFrom(s => s.AssignedTo.Email))
               .ForMember(d => d.CreatedHumanizerDate, o => o.Ignore());


            CreateMap<Citation, CitationViolationListItem>()
               .ForMember(d => d.VideoUrl, o => o.Ignore())
               .ForMember(d => d.ImageUrl, o => o.Ignore())
               .ForMember(d => d.AssignedToList, o => o.Ignore())
               .ForMember(d => d.UploadMessage, o => o.Ignore())
               .ForMember(d => d.Comment, o => o.Ignore())
               .ForMember(d => d.CommentID, o => o.Ignore())
               .ForMember(d => d.AttachmentType, o => o.Ignore())
               .ForMember(d => d.Status, o => o.Ignore())
               .ForMember(d => d.CitationComments, o => o.MapFrom(s => s.Comments))
               .ForMember(d => d.CitationAttachment, o => o.MapFrom(s => s.Attachments))
               .ForMember(d => d.CitationAuditLog, o => o.MapFrom(s => s.AuditLogs))
               .ForMember(d => d.Name, o => o.MapFrom(s => s.Violation.Name))
               .ForMember(d => d.CustomName, o => o.MapFrom(s => s.Violation.CustomName))
               .ForMember(d => d.Actions, o => o.MapFrom(s => s.Violation.Actions))
               .ForMember(d => d.CustomActions, o => o.MapFrom(s => s.Violation.CustomActions))
               .ForMember(d => d.ViolationCode, o => o.MapFrom(s => s.Violation.Code))
               .ForMember(d => d.CustomDescription, o => o.MapFrom(s => s.Violation.CustomDescription))
               .ForMember(d => d.Date, o => o.MapFrom(s => s.CreateUtc))
               .ForMember(d => d.StatusId, o => o.MapFrom(s => s.Status))
               .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreateUser.DisplayName))
               .ForMember(d => d.CreatedUserEmail, o => o.MapFrom(s => s.CreateUser.Email))
               .ForMember(d => d.CreatedByID, o => o.MapFrom(s => s.CreateUser.Id))
               .ForMember(d => d.Latitude, o => o.MapFrom(s => s.Latitude))
               .ForMember(d => d.IsPublic, o => o.Ignore())
               .ForMember(d => d.ViolationList, o => o.Ignore())
               .ForMember(d => d.ViolationId, o => o.Ignore())
               .ForMember(d => d.States, o => o.Ignore())
               .ForMember(d => d.AssignedTo, o => o.MapFrom(s => s.AssignedTo.Email))
               .ForMember(d => d.DisplayDate, o => o.Ignore())
               ;


            CreateMap<CitationAttachment, CitationAttachmentListItem>()
             .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreateUser.DisplayName))
             .ForMember(d => d.CreatedById, o => o.MapFrom(s => s.CreateUserId))
             .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreateUtc))
             .ForMember(d => d.FileName, o => o.MapFrom(s => s.Attachment.FileName))
             .ForMember(d => d.Description, o => o.MapFrom(s => s.Attachment.Description))
             .ForMember(d => d.AttachmentType, o => o.MapFrom(s => s.Attachment.AttachmentType))
             .ForMember(d => d.Key, o => o.MapFrom(s => s.Attachment.Key));


            CreateMap<CitationComment, CitationCommentListItem>()
             .ForMember(d => d.CommentID, o => o.MapFrom(s => s.Id))
             .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreateUser.DisplayName))
             .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreateUtc))
             .ForMember(d => d.CreatedById, o => o.MapFrom(s => s.CreateUserId))
             .ForMember(d => d.EnableEdit, o => o.Ignore())
             .ForMember(d => d.CreatedHumanizerDate, o => o.Ignore());

            CreateMap<CitationAuditLog, CitationAuditLogListItem>()
             .ForMember(d => d.Date, o => o.MapFrom(s => s.CreateUtc))
             .ForMember(d => d.Events, o => o.MapFrom(s => s.Event.GetEnumDescription()))
             .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreateUtc))
             .ForMember(d => d.CreateUser, o => o.MapFrom(s => s.CreateUser.DisplayName))
             .ForMember(d => d.CreatedDate, o => o.MapFrom(s => s.CreateUtc));

            CreateMap<CitationPayment, PaymentViewModel>();

            CreateMap<PaymentViewModel, CitationPayment>()
                .ForMember(d => d.Account, o => o.Ignore())
                .ForMember(d => d.Citation, o => o.Ignore())
                .ForMember(d => d.CreateUser, o => o.Ignore())
                .ForMember(d => d.UpdateUser, o => o.Ignore())
                .ForMember(d => d.CreateUser, o => o.Ignore())
                .ForMember(d => d.CreateUserId, o => o.Ignore())
                .ForMember(d => d.CreateUtc, o => o.Ignore())
                .ForMember(d => d.Timestamp, o => o.Ignore())
                .ForMember(d => d.UpdateUtc, o => o.Ignore())
                .ForMember(d => d.UpdateUserId, o => o.Ignore());
        }
    }
}
