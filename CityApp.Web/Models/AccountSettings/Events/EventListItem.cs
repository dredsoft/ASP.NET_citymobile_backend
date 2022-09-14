using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class EventListItem
    {
        public string Title { get; set; }

        public Guid Id { get; set; }

        public DateTime Created { get; set; }
    }
}
