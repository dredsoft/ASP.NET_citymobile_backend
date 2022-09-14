using CityApp.Data.Enums;
using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class ViolationListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public CommonViolationCategory Category { get; set; }
        public string TypeName { get; set; }
        public string HelpUrl { get; set; }
        public ViolationActions Actions { get; set; }
        public string Disabled { get; set; }
    }
}
