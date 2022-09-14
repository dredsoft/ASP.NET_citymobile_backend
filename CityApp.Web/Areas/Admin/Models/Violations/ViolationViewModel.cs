using CityApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class ViolationViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Type")]
        public Guid TypeId { get; set; }

        [Required]
        [Display(Name = "Category")]
        public Guid? CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "Reminder Minutes")]
        public int? ReminderMinutes { get; set; }

        [MaxLength(100)]
        [Display(Name = "Reminder Message")]
        public string ReminderMessage { get; set; }

        [Url]
        [MaxLength(250)]
        public string HelpUrl { get; set; }

        public bool Disabled { get; set; }

        public ViolationActions Actions { get; set; }
        public List<SelectListItem> Types { get; private set; } = new List<SelectListItem>();
        public List<SelectListItem> Categories { get; private set; } = new List<SelectListItem>();

        [Display(Name = "TypeName")]
        public string TypeName { get; set; }

        [Display(Name = "CategoryName")]
        public string CategoryName { get; set; }

        public ViolationRequiredFields Fields { get; set; }
    }
}
