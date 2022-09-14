using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class ViolationQuestion : AccountEntity
    {
        //Each violation will have a unique set of question
        public Guid ViolationId { get; set; }
        public Violation Violation { get; set; }

        /// <summary>
        /// This will we be the title of the question.
        /// ie. What is the name of your dog?
        /// </summary>
        [MaxLength(500)]
        public string Question { get; set; }

         /// <summary>
         /// Some question may be required
         /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Let's us know what type of question this is.
        /// Depending on the Type we could display a textbox, dropdown list, radio button or checkbox
        /// </summary>
        [Required]
        public ViolationQuestionType Type { get; set; } = ViolationQuestionType.TextField;

        /// <summary>
        /// If the quetsion type is of Single Choice or Multiple choice, we store the comma separated values of choice here. 
        /// </summary>
        [MaxLength(500)]
        public string Choices { get; set; }

        /// <summary>
        /// If a violation has multiple question, we can specify the order.
        /// </summary>
        public int Order { get; set; }
    }
}
