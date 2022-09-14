using System.ComponentModel.DataAnnotations;

namespace CityApp.Data.Models
{
    /// <summary>
    /// The Id of this field should match the ID in the Common Account. This table only exists to give referential
    /// integrity to the various account-related tables in the partition databases.
    /// </summary>
    public class Account : Entity
    {
        /// <summary>
        /// Account display name. This is a copy of CommonAccount.Name, and should only be used for debugging purposes.
        /// CommonAccount.Name is the value of record.
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }


        // ********************************************************************************************************
        // NO OTHER DATA SHOULD BE STORED ON THIS RECORD. IF YOU NEED TO ADD ACCOUNT DATA, ADD IT TO CommonAccount.
        // ********************************************************************************************************

        public override string ToString() => Name;
    }
}
