using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.User
{
    public class UserModel
    {

        public Guid Id { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }


        public IFormFile files { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8)]
        [Compare(nameof(Password))]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

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

        public virtual void SetPassword(string password)
        {
            Password = BCrypt.HashPassword(password, BCrypt.GenerateSalt());
        }

        public virtual bool CheckPassword(string password)
        {
            return BCrypt.CheckPassword(password, Password);
        }

        public string ProfileImageKey { get; set; }

        public string ImageName { get; set; }

       
    }
}
