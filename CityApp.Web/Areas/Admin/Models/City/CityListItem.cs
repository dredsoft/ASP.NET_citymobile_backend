using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Areas.Admin.Models
{
    public class CityListItem
    {
        public string Name { get; set; }

        public string State { get; set; }

        public Guid Id { get; set; }
    }
}
