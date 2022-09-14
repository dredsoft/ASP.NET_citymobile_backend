using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    /// <summary>
    /// The Id of this field should match the ID in the CommonViolationType. This table only exists to give referential
    /// integrity to the various account-related tables in the partition databases.
    /// </summary>
    public class CommonViolationType : Entity
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool Disabled { get; set; }
    }
}
