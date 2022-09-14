using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class Citation : AccountEntity
    {
        public CitationStatus Status { get; set; }

        [MaxLength(100)]
        public string ClosedReason { get; set; }

        public AccountUser AssignedTo { get; set; }
        public Guid? AssignedToId { get; set; }

        public Guid? ViolationId { get; set; }
        public Violation Violation { get; set; }

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

        [MaxLength(250)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [MaxLength(150)]
        public string Street { get; set; }

        [MaxLength(50)]
        public string PostalCode { get; set; }        


        [MaxLength(17)]
        public string VinNumber { get; set; }


        [MaxLength(100)]
        public string LicenseState { get; set; }

        public double? FineAmount { get; set; }

        public double? Balance { get; set; }


        #region Violator Info

        public string ViolatorFirstName { get; set; }
        public string ViolatorLastName { get; set; }
        public string ViolatorAddress1 { get; set; }
        public string ViolatorAddress2 { get; set; }
        public string ViolatorCity { get; set; }
        public string ViolatorState { get; set; }
        public string ViolatorZip { get; set; }
        public string ViolatorCountry { get; set; }

        #endregion

        #region Closeout Info
        //public DateTime? PaymentAttemptedDate { get; set; }
        //public DateTime? ContestedDate { get; set; }
        //public DateTime? WarningOptionSelected { get; set; }
        #endregion

        /// <summary>
        /// Let's us know when the Evidence Package was delivered to the Ticket Processing Partner
        /// </summary>
        public DateTime? EvidencePackageCreated { get; set; }


        /// <summary>
        /// Where in S3 this is located
        /// </summary>
        public string EvidencePackageKey{ get; set; }

        public List<ViolationQuestionAnswer> Answers { get; set; } = new List<ViolationQuestionAnswer>();
        public List<CitationComment> Comments { get; set; } = new List<CitationComment>();
        public List<CitationAttachment> Attachments { get; set; } = new List<CitationAttachment>();
        public List<CitationAuditLog> AuditLogs { get; set; } = new List<CitationAuditLog>();
        public List<CitationPayment> Payments { get; set; } = new List<CitationPayment>();
        public List<WarningQuizResponse> WarningEventResponses { get; set; } = new List<WarningQuizResponse>();
    }
}
