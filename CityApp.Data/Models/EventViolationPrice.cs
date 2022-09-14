using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    /// <summary>
    /// Events can have special pricing.  This class is used to hold that special pricing. 
    /// </summary>
    public class EventViolationPrice : AccountEntity
    {
        public Guid EventId { get; set; }
        public Event Event { get; set; }

        public Guid ViolationId { get; set; }
        public Violation Violation { get; set; }

        public double? Fee { get; set; }
    }
}
