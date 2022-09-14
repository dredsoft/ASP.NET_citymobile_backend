using System.ComponentModel.DataAnnotations;

namespace CityApp.Data.Models
{
    /// <summary>
    /// This class represents the database
    /// </summary>
    public class Partition : Entity
    {
        /// <summary>
        /// Connection String Name
        /// </summary>
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
        public string ConnectionString { get; set; }
    }
}
