using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class ViolationQuestionAnswer : AccountEntity
    {
        //This tells us which Citation this answer belongs to
        public Citation Citation { get; set; }
        public Guid CitationId { get; set; }

        //This tells which question the answer was from
        public ViolationQuestion ViolationQuestion { get; set; }
        public Guid ViolationQuestionId { get; set; }



        //This tells us the original question.  It's important to have this because sometimes the user may change the question in the ViolationQuestion.
        //It also makes displaying the question and answer very easy.
        [MaxLength(500)]
        public string Question { get; set; }

        //This is the answer to the question.
        [MaxLength(500)]
        public string Answer { get; set; }

        public ViolationQuestionType Type { get; set; }
    }
}
