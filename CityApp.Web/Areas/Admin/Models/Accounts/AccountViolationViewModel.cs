using CityApp.Data.Enums;
using CityApp.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class AdminAccountViolationViewModel
    {
        public Guid? Id { get; set; }
        public Guid AccountId { get; set; }

        public Guid CategoryId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }


        [MaxLength(250)]
        public string HelpUrl { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        //public string WarningQuizUrl { get; set; }

        public double? Fee { get; set; }

        [Display(Name = "Reminder Minutes")]
        public int? ReminderMinutes { get; set; }

        [MaxLength(100)]
        public string ReminderMessage { get; set; }

        //Actions from CommonViolation
        public ViolationActions Actions { get; set; }

        //Required Fields from CommonViolation
        public ViolationRequiredFields RequiredFields { get; set; }

        public SelectList Categories { get; set; }
    }
}
