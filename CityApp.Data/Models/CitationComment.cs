using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class CitationComment : AccountEntity
    {

    
        public Guid CitationId { get; set; }
        public Citation Citation { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }

        public bool IsPublic { get; set; }
    }
}
