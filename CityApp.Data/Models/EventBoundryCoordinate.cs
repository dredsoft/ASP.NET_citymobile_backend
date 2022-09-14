using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class EventBoundaryCoordinate : AccountEntity
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set;  }

        public int Order { get; set; }
    }
}
