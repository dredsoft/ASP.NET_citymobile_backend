using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Models
{
    public class AccountViolationType
    {
        [Display(Name = "Type")]
        [Required]
        public Guid TypeId { get; set; }

        [Display(Name = "TypeName")]
        public string TypeName { get; set; }
        public bool IsCheckedViolation { get; set; } = false;
    }
}
