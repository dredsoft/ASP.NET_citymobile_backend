using CityApp.Data.Enums;
using CityApp.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models.Citation
{
    public class CitationListModel
    {
        public Guid Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CitationStatus Status { get; set; }

        public AccountUser AssignedTo { get; set; }
        public Guid? AssignedToId { get; set; }

        public string AssignedToFullName { get; set; }

        public Guid? ViolationId { get; set; }

        public string ViolationName { get; set; }

        public string ViolationCode { get; set; }


        //public Violation Violation { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        [MaxLength(25)]
        public string LicensePlate { get; set; }

        [MaxLength(255)]
        public string LocationDescription { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public long CitationNumber { get; set; }

        [MaxLength(50)]
        public string VehicleColor { get; set; }

        [MaxLength(50)]
        public string VehicleMake { get; set; }

        [MaxLength(50)]
        public string VehicleModel { get; set; }

        [MaxLength(50)]
        public string VehicleType { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public DateTime CreateUtc { get; set; }

        public string VinNumber { get; set; }

        [NotMapped]
        public string Thumbnail
        {
            get
            {
                var defaultThumbnailURL = "";

                if (CitationAttachment.Any())
                {
                    var imageWithThumb = CitationAttachment
                        .Where(m => m.AttachmentType == CitationAttachmentType.Image)
                        .Where(m => m.Key.Contains("thumb")).FirstOrDefault();

                    if(imageWithThumb != null)
                    {
                        defaultThumbnailURL = imageWithThumb.Key;
                    }
                }

                return defaultThumbnailURL;
            }
        }

        public List<CitationCommentModel> Comments { get; set; } = new List<CitationCommentModel>();
        public List<AttachmentModel> CitationAttachment { get; set; } = new List<AttachmentModel>();

    }
}
