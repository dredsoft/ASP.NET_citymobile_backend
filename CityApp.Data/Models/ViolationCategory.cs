using CityApp.Data.Models.Interface;
using System;
using System.ComponentModel.DataAnnotations;

namespace CityApp.Data.Models
{
    /// <summary>
    /// </summary>
    public class ViolationCategory : Entity, IHasAccount
    {
        /// <summary>
        /// Reference to the CommonViolationCategory Entity.  Nullable becuase accounts can create their own violation Categories
        /// </summary>
        public Guid? CommonCategoryId { get; set; }

        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        public ViolationType Type { get; set; }
        public Guid TypeId { get; set; }

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
