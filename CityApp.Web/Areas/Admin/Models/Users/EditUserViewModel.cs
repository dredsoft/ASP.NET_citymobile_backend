using CityApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Areas.Admin.Models.Users
{
    public class EditUserViewModel
    {

        public Guid Id { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [MinLength(8)]
        [Display(Name = "Password")]
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


        [Display(Name = "System Permission")]
        [Required]
        public SystemPermissions Permission { get; set; }

        [NotMapped]
        public virtual string FullName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(MiddleName))
                {
                    return FirstName + " " + LastName;
                }
                else
                {
                    return FirstName + $" {MiddleName} " + LastName;
                }
            }
        }

        [NotMapped]
        public virtual string Description
        {
            get
            {
                return !string.IsNullOrWhiteSpace(FullName)
                    ? FullName
                    : Email;
            }
        }

        /// <summary>
        /// TOTP Secret Key
        /// </summary>
        [MaxLength(100)]
        public string TotpSecret { get; set; }

        /// <summary>
        /// TOTP Recovery in case they can't produce an OTP for some reason. If this is used, TOTP should be disabled and need to be re-enabled.
        /// </summary>
        [MaxLength(50)]
        public string TotpRecovery { get; set; }

        /// <summary>
        /// Number of failed attempts since last reset. Set to 0 on successful login.
        /// </summary>
        public int FailedSinceResetCount { get; set; }

        /// <summary>
        /// First failed attempt. Set to null on successful login.
        /// </summary>
        public DateTime? FailedResetUtc { get; set; }

        /// <summary>
        /// User's last session ID if we want to discourage users from using the same login.
        /// </summary>
        [MaxLength(50)]
        public string LastSession { get; set; }

        public virtual void SetPassword(string password)
        {
            Password = BCrypt.HashPassword(password, BCrypt.GenerateSalt());
        }

        public virtual bool CheckPassword(string password)
        {
            return BCrypt.CheckPassword(password, Password);
        }

        [MaxLength(100)]
        public string Token { get; set; }

        public DateTime? TokenUtc { get; set; }



        public List<SelectListItem> systemPermission { get; private set; } = new List<SelectListItem>();
    }
}
