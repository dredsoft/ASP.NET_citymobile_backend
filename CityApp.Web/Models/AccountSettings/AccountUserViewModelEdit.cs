using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.AccountSettings
{
    public class AccountUserViewModelEdit
    {
        public Guid? Id { get; set; }

        public Guid UserId { get; set; }

        [MaxLength(255)]
      
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [MaxLength(50)]
      
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [MaxLength(50)]
       
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public bool Disabled { get; set; }


        public string FullName => $"{FirstName} {LastName}";


        public AccountPermissions Permissions { get; set; }

        [NotMapped]
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FullName))
                {
                    return Email;
                }
                else
                {
                    return FullName;
                }
            }
        }
    }
}
