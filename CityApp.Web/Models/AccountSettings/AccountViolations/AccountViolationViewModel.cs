using CityApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.AccountSettings
{
    public class AccountViolationViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Recommended Name")]
        public string Name { get; set; }

        [Display(Name = "Recommended Description")]
        public string Description { get; set; }

        [Display(Name = "Recommended Help Url")]
        public string HelpUrl { get; set; }

        [Display(Name = "Recommended Actions")]
        public ViolationActions Actions { get; set; }

        [MaxLength(100)]
        [Display(Name = "Custom Name")]
        public string CustomName { get; set; }

        [MaxLength(500)]
        [Display(Name = "Custom Description")]
        public string CustomDescription { get; set; }

        [Url]
        [MaxLength(250)]
        [Display(Name = "Custom Help Url")]
        public string CustomHelpUrl { get; set; }

        [Display(Name = "Custom Action")]
        public ViolationActions CustomActions { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        public List<SelectListItem> Types { get; private set; } = new List<SelectListItem>();
        public List<SelectListItem> Categories { get; private set; } = new List<SelectListItem>();

        [Display(Name = "Type Name")]
        public string TypeName { get; set; }

        public string Question { get; set; }

        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }

        [Display(Name = "Recommended Required Fields")]
        public ViolationRequiredFields Fields { get; set; }

        [Display(Name = "Custom Action")]
        public ViolationRequiredFields CustomRequiredFields { get; set; }

        [Display(Name = "Fee")]
        public double? Fee { get; set; }

        [Display(Name = "Reminder Minutes")]
        public int? ReminderMinutes { get; set; }

        [MaxLength(100)]
        [Display(Name = "Reminder Message")]
        public string ReminderMessage { get; set; }

        public Guid QuestionId { get; set; }

        public bool IsRequired { get; set; }
        public List<ViolationQuestionListItem> ViolationQuestion { get; set; }
    }
}
