using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Areas.Admin.Models.Partitions
{
    public class PartitionViewModel
    {

        public Guid Id { get; set; }

        /// <summary>
        /// Connection String Name
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Do not create and new users or accounts in this partition.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// When creating new accounts, use the Partition with the lowest Occupancy that is not Disabled.
        /// </summary>
        public long Occupancy { get; set; }

        [Required]
        [MaxLength(1024)]
        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; }
    }
}
