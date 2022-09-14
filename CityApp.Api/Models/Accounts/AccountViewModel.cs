using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class AccountViewModel
    {
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Guid AccountId { get; set; }
        public long AccountNumber { get; set; }
        public DateTime? ExpirationUtc { get; set; }
        public bool Disabled { get; set; }
        public decimal Distance { get; set; }
    }
   
}
