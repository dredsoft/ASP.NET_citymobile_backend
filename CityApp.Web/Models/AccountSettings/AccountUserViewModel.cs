using CityApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.AccountSettings
{
    public class AccountUserViewModel
    {
        public Guid? Id { get; set; }

        public Guid? UserId { get; set; }

        [MaxLength(255)]
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [MaxLength(50)]
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [MaxLength(50)]
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        public string FullName => $"{FirstName} {LastName}";


        public AccountPermissions Permissions { get; set; }

        public Guid? VendorId { get; set; }


        public bool IsVendor { get; set; }

        public SystemPermissions systemPermission { get; set; }

        
        public List<SelectListItem> Vendors { get; private set; } = new List<SelectListItem>();

        public List<SelectListItem> SystemPermissions { get; private set; } = new List<SelectListItem>();


    }
}
