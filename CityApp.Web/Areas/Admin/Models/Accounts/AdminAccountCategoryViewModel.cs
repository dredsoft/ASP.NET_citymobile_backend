using CityApp.Data.Enums;
using CityApp.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class AdminAccountCategoryViewModel
    {
        public Guid? Id { get; set; }
        public Guid AccountId { get; set; }

        [Display( Name = "Type")]
        public Guid TypeId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public SelectList Types { get; set; }
    }
}
