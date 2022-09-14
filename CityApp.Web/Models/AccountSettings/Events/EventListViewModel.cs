using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class EventListViewModel : ListBase
    {
        public List<EventListItem> EventListItem { get; set; }
    }
}
