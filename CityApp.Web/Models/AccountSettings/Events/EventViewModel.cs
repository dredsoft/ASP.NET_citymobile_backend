using AutoMapper;
using CityApp.Areas.Admin.Models;
using CityApp.Web.Models.AccountSettings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class EventViewModel
    {
        [Required]
        [MaxLength(255)]
        [Display(Name ="Event Name")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Event Info")]
        public string Body { get; set; }

        [Display(Name = "Start Time")]
        public DateTime? Start { get; set; }

        [Display(Name = "End Time")]
        public DateTime? End { get; set; }

        public Guid Id { get; set; }

        [IgnoreMap]
        public decimal Latitude { get; set; }
        [IgnoreMap]
        public decimal Longitude { get; set; }

        public List<AccountViolationListItem> Violations { get; set; } = new List<AccountViolationListItem>();
        public List<EventPricingViewModel> EventViolationPrices { get; set; } = new List<EventPricingViewModel>();

    }
}
