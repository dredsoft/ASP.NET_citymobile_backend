using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.User
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

    }

    public class ForgetPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class ResetPassword
    {

        [EmailAddress]
        public string Email { get; set; }

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

        public string Code { get; set; }

        public string Id { get; set; }

    }
}
