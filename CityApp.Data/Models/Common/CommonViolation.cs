using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class CommonViolation : Entity
    {
        public CommonViolationCategory Category { get; set; }
        public Guid? CategoryId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(250)]
        public string HelpUrl { get; set; }

        public bool Disabled { get; set; }

        public int? ReminderMinutes { get; set; }

        [MaxLength(100)]
        public string ReminderMessage { get; set; }

        //public string WarningQuizUrl { get; set; }

        public ViolationActions Actions { get; set; }

        public ViolationRequiredFields RequiredFields { get; set; }

    }
}
