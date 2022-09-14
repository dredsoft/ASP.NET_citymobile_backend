using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Services.Models
{

    public class CitationReceiptModel
    {
        public long CitationNumber { get; set; }
        public long AccountNumber { get; set; }
        public string LicensePlate { get; set; }
        public string LicenseState { get; set; }
        public string AccountName { get; set; }
        public string AccountCity { get; set; }
        public string AccountState { get; set; }
        public string ViolationCode { get; set; }
        public string ViolationTitle { get; set; }
        public string ViolationState { get; set; }
        public string ViolationCity { get; set; }
        public decimal ViolationLatitude { get; set; }
        public decimal ViolationLongitude { get; set; }
        public DateTime ViolationCreatedUTC { get; set; }
    }
}
