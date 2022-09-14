using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class AccViolationListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ViolationCategory Category { get; set; }
        public string TypeName { get; set; }
        public string HelpUrl { get; set; }
        public bool Disabled { get; set; }
    }
}
