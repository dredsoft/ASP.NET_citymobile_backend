using CityApp.Data.Enums;
using CityApp.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class ViolationsModel : ListBase
    {
        
        public ViolationActions Actions { get; set; }
        public ViolationActions CustomActions { get; set; }

        [Display(Name = "CategoryName")]
        public string CategoryName { get; set; }
        public List<SelectListItem> Categories { get; private set; } = new List<SelectListItem>();

        public List<ViolationsListModel> Violation { get; set; } = new List<ViolationsListModel>();

        public List<SelectListItem> Types { get; private set; } = new List<SelectListItem>();
    }
}
