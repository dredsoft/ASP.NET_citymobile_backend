using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.User
{
    public class CreatePassword
    {
        public string AccountName { get; set; }

        public string InvitedEmail { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int Permission { get; set; }
        public SystemPermissions SystemPermission { get; set; } = SystemPermissions.None;

        public Guid AccountId { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8)]
        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8)]
        [Compare(nameof(Password))]
        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public Guid VendorId { get; set; }
    }
}
