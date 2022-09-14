using CityApp.Data.Models.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    /// <summary>
    /// </summary>
    public class ViolationType : Entity, IHasAccount
    {
        public Guid CommonViolationTypeId { get; set; }

        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string CustomName { get; set; }

        [MaxLength(500)]
        public string CustomDescription { get; set; }


        public bool Disabled { get; set; }

    }
}
