using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Models
{
    public class CachedAccountViolations
    {
        public Guid ViolationId { get; set; }
        public string ViolationName { get; set; }

        public Guid ViolationCategoryId { get; set; }
        public string ViolationCategoryName { get; set; }

        public Guid ViolationTypeId { get; set; }
        public string ViolationTypeName { get; set; }

    }
}
