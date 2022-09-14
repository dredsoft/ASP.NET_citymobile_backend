using CityApp.Data.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class UserProfileModel
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

        [JsonConverter(typeof(StringEnumConverter))]
        public SystemPermissions Permission { get; set; }

        public string ProfileImageUrl { get; set; }

        public string ProfileImageKey { get; set; }


    }
}
