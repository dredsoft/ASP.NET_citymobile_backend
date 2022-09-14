using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class CitationReminder : AccountEntity
    {
        public Guid CitationId { get; set; }
        public Citation Citation { get; set; }

        public string Message { get; set; }
        public DateTime? SentDateUTC { get; set; }
        public DateTime DeliveryDateUTC { get; set; }
    }
}
