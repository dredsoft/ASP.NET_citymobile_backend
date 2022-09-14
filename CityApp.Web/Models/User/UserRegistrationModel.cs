using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.User
{
    public class UserRegisterModel
    {
        [Required]
        [MaxLength(255)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8)]
        [Required]
        public string Password { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please agree to the terms and conditions")]
        public bool IsChecked { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8)]
        [Compare(nameof(Password))]
        [Required]
        [Display(Name = "Confirm Password")]
        public string PasswordAgain { get; set; }
    }
}
