using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class EventCoordinatesViewModel
    {
        public Guid EventId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public int Order { get; set; }
    }
}
