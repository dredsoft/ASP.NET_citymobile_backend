using AutoMapper;
using CityApp.Api.Models;
using CityApp.Api.Models.Citation;
using CityApp.Common.Models;
using CityApp.Data.Models;


namespace CityApp.Api.MappingProfiles
{
    public class CitationProfile : Profile
    {
        public CitationProfile()
        {
            CreateMap<CitationRequestModel, CitationResponseModel>().ReverseMap();

            CreateMap<CitationRequestModel, Citation>()
                .ForMember(d => d.Address, o => o.Ignore())
                .ForMember(d => d.City, o => o.Ignore())
                .ForMember(d => d.State, o => o.Ignore())
                .ForMember(d => d.Street, o => o.Ignore())
                .ForMember(d => d.PostalCode, o => o.Ignore())
                .ForMember(d => d.Attachments, o => o.Ignore())
                .ForMember(d => d.ViolatorFirstName, o => o.Ignore())
                .ForMember(d => d.ViolatorLastName, o => o.Ignore())
                .ForMember(d => d.ViolatorAddress1, o => o.Ignore())
                .ForMember(d => d.ViolatorAddress2, o => o.Ignore())
                .ForMember(d => d.ViolatorCity, o => o.Ignore())
                .ForMember(d => d.ViolatorState, o => o.Ignore())
                .ForMember(d => d.ViolatorZip, o => o.Ignore())
                .ForMember(d => d.ViolatorCountry, o => o.Ignore())
                .ForMember(d => d.EvidencePackageCreated, o => o.Ignore())
                .ForMember(d => d.EvidencePackageKey, o => o.Ignore())
                .ForMember(d => d.Answers, o => o.Ignore())
                .ForMember(d => d.AuditLogs, o => o.Ignore())
                .ForMember(d => d.AssignedToId, o => o.Ignore())
                .ForMember(d => d.AssignedTo, o => o.Ignore())
                .ForMember(d => d.Violation, o => o.Ignore())
                .ForMember(d => d.Comments, o => o.Ignore())
                .ForMember(d => d.AccountId, o => o.Ignore())
                .ForMember(d => d.Account, o => o.Ignore())
                .ForMember(d => d.UpdateUser, o => o.Ignore())
                .ForMember(d => d.CreateUser, o => o.Ignore())
                .ForMember(d => d.CreateUserId, o => o.Ignore())
                .ForMember(d => d.CreateUtc, o => o.Ignore())
                .ForMember(d => d.UpdateUserId, o => o.Ignore())
                .ForMember(d => d.UpdateUtc, o => o.Ignore())
                .ForMember(d => d.Timestamp, o => o.Ignore())
                .ForMember(d => d.Balance, o => o.Ignore())
                .ForMember(d => d.Payments, o => o.Ignore())
                .ForMember(d => d.ClosedReason, o => o.Ignore())
                .ForMember(d => d.WarningEventResponses, o => o.Ignore())
            .ForMember(d => d.FineAmount, o => o.Ignore());


            CreateMap<Citation, CitationResponseModel>();

            CreateMap<CitationComment, CitationCommentModel>()
                .ForMember(d => d.Citations, o => o.Ignore());

            CreateMap<Citation, CitationListModel>()
                .ForMember(d => d.AssignedToFullName, o => o.MapFrom(s => s.AssignedTo.FullName))
                 .ForMember(d => d.ViolationName, o => o.MapFrom(s => s.Violation.Name))
                 .ForMember(d => d.ViolationCode, o => o.MapFrom(s => s.Violation.Code))
             .ForMember(d => d.CitationAttachment, o => o.MapFrom(s => s.Attachments))
             .ForMember(d => d.AssignedTo, o => o.Ignore());


            CreateMap<CitationAttachment, AttachmentResponse>();



        }

    }
}
