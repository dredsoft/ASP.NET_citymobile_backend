using CityApp.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityApp.Data.Models
{
    public class CommonUser : Entity
    {
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Password { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

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

        //TODO: This property should not belong here.
        [NotMapped]
        public string City { get; set; }

        //TODO: This property should not belong here.
        [NotMapped]
        public string Account { get; set; }

        //TODO: This property should not belong here.
        [NotMapped]
        public string Partition { get; set; }

        //TODO: This property should not belong here.
        [NotMapped]
        public CommonAccount CommonAccount { get; set; }

        [MinLength(10)]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Where the file is located in storage
        /// </summary>
        [MaxLength(255)]
        public string ProfileImageKey { get; set; }
    }
}
