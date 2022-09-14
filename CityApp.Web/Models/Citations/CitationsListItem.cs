using CityApp.Data.Enums;
using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Citations
{
    public class CitationsListItem
    {

        public Guid Id { get; set; }

        public CitationStatus Status { get; set; }
        public Violation Violation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime Created { get; set; }
        public long CitationNumber { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string CreatedHumanizerDate { get; set; }
        public string LicensePlate { get; set; }
        public double? FineAmount { get; set; }

        public string Postalcode { get; set; }

        public string Street { get; set; }

        public double Balance { get {
                if(FineAmount.HasValue)
                {
                    return FineAmount.Value - Payments.Sum(m => m.CitationFineAmount);
                }
                else
                {
                    return 0f;
                }
            } }

        public List<CitationPayment> Payments { get; set; } = new List<CitationPayment>();



    }
}
