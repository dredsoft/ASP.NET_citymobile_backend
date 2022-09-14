using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class Counter : AccountEntity
    {
        [MaxLength(50), Required]
        public string Name { get; set; }

        [Required]
        public long NextValue { get; set; }
       

    }
}
