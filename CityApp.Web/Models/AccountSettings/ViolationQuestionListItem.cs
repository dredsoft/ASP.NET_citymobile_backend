using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class ViolationQuestionListItem
    {
        public Guid QuestionID { get; set; }
        public string Question { get; set; }
        public Guid CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool EnableEdit { get; set; }
        public string CreatedHumanizerDate { get; set; }

        public int Order { get; set; }

        public string Type { get; set; }

        public string Choices { get; set; }

        public bool IsRequired { get; set; }
    }

    public class OrderList
    {
        public Guid questionID { get; set; }
        public int order { get; set; }
    }
}
