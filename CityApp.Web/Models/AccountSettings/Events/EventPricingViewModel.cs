using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class EventPricingViewModel
    {
        public Guid? Id { get; set; }

        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public Guid ViolationId { get; set; }
        public Violation Violation { get; set; }

        public double? Fee { get; set; }
    }
}
