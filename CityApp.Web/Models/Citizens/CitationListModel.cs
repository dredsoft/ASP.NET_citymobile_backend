using CityApp.Data.Enums;
using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class CitationListModel
    {
        public Guid Id { get; set; }

        public CitationStatus Status { get; set; }

        public Guid AccountId { get; set; }


        public Guid? ViolationId { get; set; }

        public string ViolationName { get; set; }

        public string ViolationCode { get; set; }


        //public Violation Violation { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        [Required]
        public long CitationNumber { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public DateTime CreateUtc { get; set; }

        public List<AttachmentModel> CitationAttachment { get; set; }

    }
}
