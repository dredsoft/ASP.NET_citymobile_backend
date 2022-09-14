using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityApp.Data.Models
{
    /// <summary>
    /// This Id for this AccountUser should be exactly the same as the CommonUser.Id
    /// Common User holds the username and password. 
    /// </summary>
    public class AccountUser : Entity
    {
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(20)]
        public string BadgeNumber { get; set; }

        [MaxLength(20)]
        public string Agency { get; set; }

        public string FullName => $"{FirstName} {LastName}";


        [NotMapped]
        public string DisplayName
        {
            get
            {
                if(string.IsNullOrWhiteSpace(FirstName))
                {
                    return Email;
                }
                else
                {
                    return FullName;
                }
            }
        }

        /// <summary>
        /// Where the file is located in storage
        /// </summary>
        [MaxLength(255)]
        public string ProfileImageKey { get; set; }

    }
}
