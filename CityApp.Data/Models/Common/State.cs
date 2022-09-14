using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class State : Entity
    {
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
