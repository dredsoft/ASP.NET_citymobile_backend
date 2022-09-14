using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class WarningQuizResponse : AccountEntity 
    {
        public Guid CitationId { get; set; }
        public Citation Citation { get; set; }

        public string Payload { get; set; }


    }
}
