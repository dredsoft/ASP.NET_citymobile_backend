using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class ViolationCategoryListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TypeName { get; set; }
    }
}
