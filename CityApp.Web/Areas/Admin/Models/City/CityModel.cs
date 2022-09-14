using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Areas.Admin.Models
{
    public class CityModel
    {
        public Guid Id { get; set; }

        [Required]
        public String Name { get; set; }

        [MaxLength(100)]
        [Display(Name = "Country")]
        public String County { get; set; }
        [MaxLength(25)]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Time Zone")]
        [MaxLength(100)]
        public string TimeZone { get; set; }

        [Required]
        [MaxLength(50)]
        public string State { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        [Required]
        [MaxLength(2)]
        [Display(Name = "State Code")]
        public string StateCode { get; set; }
    }
}
