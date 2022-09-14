﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class ChangePassword
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
    }
}
