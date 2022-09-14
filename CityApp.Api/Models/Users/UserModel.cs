using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class UserModel
    {
        [MaxLength(255)]
        [Required]
        public string Email { get; set; }

        [MaxLength(50)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [MaxLength(50)]
        [Required]
        public string LastName { get; set; }

        [MinLength(10)]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        public Guid Id { get; set; }

        [MaxLength(100)]
        public string Password { get; set; }

        public string profileImageKey { get; set; }

        public virtual void SetPassword(string password)
        {
            Password = BCrypt.HashPassword(password, BCrypt.GenerateSalt());
        }

        public virtual bool CheckPassword(string password)
        {
            return BCrypt.CheckPassword(password, Password);
        }

    }
}
