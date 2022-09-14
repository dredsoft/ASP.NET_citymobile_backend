using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class Event : AccountEntity
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public List<EventViolationPrice> EventViolationPrices { get; set; } = new List<EventViolationPrice>();
        public List<EventBoundaryCoordinate> EventBoundaryCoordinates { get; set; } = new List<EventBoundaryCoordinate>();
    }
}
