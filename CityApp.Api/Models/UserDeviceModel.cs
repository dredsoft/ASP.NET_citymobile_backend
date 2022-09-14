using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class UserDeviceModel
    {
        [MaxLength(100)]
        [Required]
        [Display(Name = "Device Name")]
        public string DeviceName { get; set; }

        [MaxLength(500)]
        [Required]
        [Display(Name = "Device Token")]
        public string DeviceToken { get; set; }

        [MaxLength(100)]
        [Required]
        [Display(Name = "Device Type")]
        public string DeviceType { get; set; }

        public bool IsDisabled { get; set; }

        [MaxLength(500)]
        public string DevicePublicKey { get; set; }
    }
}
