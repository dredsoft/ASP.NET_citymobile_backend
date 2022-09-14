using CityApp.Data.Enums;
using CityApp.Data.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    /// <summary>
    /// </summary>
    public class Violation : Entity, IHasAccount
    {
        /// <summary>
        /// Reference to the CommonViolation Entity.  Nullable becuase accounts can create their own violations
        /// </summary>
        public Guid? CommonViolationId { get; set; }

        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        public ViolationCategory Category { get; set; }
        public Guid? CategoryId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string CustomName { get; set; }


        [MaxLength(500)]
        public string Description { get; set; }
        [MaxLength(500)]
        public string CustomDescription { get; set; }

        [MaxLength(250)]
        public string HelpUrl { get; set; }
        [MaxLength(250)]
        public string CustomHelpUrl { get; set; }


        [MaxLength(50)]
        public string Code { get; set; }

        public bool Disabled { get; set; }

        public double? Fee { get; set; }

        public int? ReminderMinutes { get; set; }

        [MaxLength(100)]
        public string ReminderMessage { get; set; }

        //public string WarningQuizUrl { get; set; }

        //[MaxLength(250)]
        //public string CustomWarningQuizUrl { get; set; }

        /// <summary>
        /// Let's us know when the Evidence Package was delivered to the Ticket Processing Partner
        /// TODO: This is in the wrong spot.  It needs to be in Citations
        /// </summary>
        public DateTime? EvidencePackageDelivered { get; set; }

        //Actions from CommonViolation
        public ViolationActions Actions { get; set; }
        public ViolationActions CustomActions { get; set; }

        //Required Fields from CommonViolation
        public ViolationRequiredFields RequiredFields { get; set; }
        public ViolationRequiredFields CustomRequiredFields { get; set; }

        public List<ViolationQuestion> Questions { get; set; } = new List<ViolationQuestion>();
    }
}
