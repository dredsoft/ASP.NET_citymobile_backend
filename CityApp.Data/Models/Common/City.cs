using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class City : Entity
    {
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string County { get; set; }

        [MaxLength(2)]
        public string StateCode { get; set; }

        [MaxLength(25)]
        public string Type { get; set; }


        [MaxLength(100)]
        public string TimeZone { get; set; }

        [MaxLength(50)]
        public string State { get; set; }     

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }


    }
}
