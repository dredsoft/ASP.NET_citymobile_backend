using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class ViolationQuestionsListItem
    {
        public Guid QuestionID { get; set; }
        public string Question { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool EnableEdit { get; set; }
        public string CreatedHumanizerDate { get; set; }

        public bool IsRequired { get; set; }
    }
}
