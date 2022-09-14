using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class CommonAccountViolationType : Entity
    {
        public CommonViolationType ViolationType { get; set; }
        public Guid? ViolationTypeId { get; set; }

        public CommonAccount Account { get; set; }
        public Guid AccountId { get; set; }
    }
}
