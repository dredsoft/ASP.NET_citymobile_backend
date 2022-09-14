using AutoMapper;
using CityApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CityApp.Data.Models;

namespace CityApp.Web.Models
{
    public class CitationViolationListItem
    {

        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public string CustomName { get; set; }
        public DateTime Date { get; set; }
        public string DisplayDate { get; set; }

        public ViolationActions Actions { get; set; }
        public ViolationActions CustomActions { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Guid? AssignedToId { get; set; }

        public CitationStatus StatusId { get; set; }

        public string AssignedTo { get; set; }
        public long CitationNumber { get; set; }
        public string LicensePlate { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleType { get; set; }
        public string VehicleColor { get; set; }

        public string ViolationCode { get; set; }
        [Display(Name ="Description/Comments")]
        public string Description { get; set; }

        [Display(Name = "Location Details")]
        public string LocationDescription { get; set; }
        public string CustomDescription { get; set; }
        public string CreatedBy { get; set; }

        public Guid CreatedByID { get; set; }

        public string CreatedUserEmail { get; set; }

        public string VideoUrl { get; set; }
        public string ImageUrl { get; set; }

        public string UploadMessage { get; set; }

        public string VinNumber { get; set; }

        public bool IsPublic { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Street { get; set; }

        [Display(Name = "Add a comment")]
        public string Comment { get; set; }
        public Guid CommentID { get; set; }

        [IgnoreMap]
        public bool IsEdit { get; set; }
       
        public Guid? ViolationId { get; set; }

        [IgnoreMap]
        public Guid? VideoAttachmentId{ get; set; }

        public string LicenseState { get; set; }

        public double? FineAmount { get; set; }

        [Display(Name = "First Name")]
        public string ViolatorFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string ViolatorLastName { get; set; }

        [Display(Name = "Address 1")]
        public string ViolatorAddress1 { get; set; }

        [Display(Name = "Address 2")]
        public string ViolatorAddress2 { get; set; }

        [Display(Name = "City")]
        public string ViolatorCity { get; set; }

        [Display(Name = "State")]
        public string ViolatorState { get; set; }

        [Display(Name = "Zip")]
        public string ViolatorZip { get; set; }

        [Display(Name = "County")]
        public string ViolatorCountry { get; set; }

        /// <summary>
        /// Let's us know when the Evidence Package was delivered to the Ticket Processing Partner
        /// </summary>
        public DateTime? EvidencePackageCreated { get; set; }


        /// <summary>
        /// Where in S3 this is located
        /// </summary>
        public string EvidencePackageKey { get; set; }

        public List<CitationCommentListItem> CitationComments { get; set; }

        public List<CitationAttachmentListItem> CitationAttachment { get; set; }

        public List<CitationAuditLogListItem> CitationAuditLog { get; set; }
        public CitationAttachmentType AttachmentType { get; set; }
        public ViolationRequiredFields ViolationCustomRequiredFields { get; set; }

        public List<SelectListItem> Status { get; private set; } = new List<SelectListItem>();

        public List<SelectListItem> AssignedToList { get; private set; } = new List<SelectListItem>();

        public List<SelectListItem> ViolationList { get; private set; } = new List<SelectListItem>();

        public List<SelectListItem> States { get; private set; } = new List<SelectListItem>();
        public List<CitationPayment> Payments { get; set; } = new List<CitationPayment>();

        /// <summary>
        /// Display Name instead of CustomName if it is null or empty
        /// </summary>
        [IgnoreMap]
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CustomName))
                {
                    return Name;
                }
                else
                {
                    return CustomName;
                }
            }
        }

        /// <summary>
        /// Display Actions instead of CustomActions if it is null or empty
        /// </summary>
        [IgnoreMap]
        public string DisplayActions
        {
            get
            {
                if (CustomActions != 0)
                {
                    return CustomActions.ToString();
                }
                else if (Actions != 0)
                {
                    return Actions.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Display Description instead of CustomDescription if it is null or empty
        /// </summary>
        [IgnoreMap]
        public string DisplayDescription
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CustomDescription))
                {
                    return Description;
                }
                else
                {
                    return CustomDescription;
                }
            }
        }


    }
}