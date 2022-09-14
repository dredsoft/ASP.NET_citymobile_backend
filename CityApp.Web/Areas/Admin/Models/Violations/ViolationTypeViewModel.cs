using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class ViolationTypeViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Type")]
        [Required]
        public Guid TypeId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool Disabled { get; set; }

        public List<SelectListItem> Types { get; private set; } = new List<SelectListItem>();

        [Display(Name = "TypeName")]
        public string TypeName { get; set; }
    }
}
