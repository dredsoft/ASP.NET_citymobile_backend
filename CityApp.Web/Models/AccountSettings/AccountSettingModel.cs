using CityApp.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{

    public class AccountSettingModel
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        [Required]
        [Display(Name = "Account Name")]
        public string Name { get; set; }

        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        [Display(Name = "Contact Number")]
        [MinLength(10)]
        [MaxLength(15)]
        public string ContactNumber { get; set; }


        public IFormFile files { get; set; }

        public string ImageName { get; set; }

        [MaxLength(255)]       
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string ContactEmail { get; set; }

        [Display(Name = "Owner")]
        public Guid? OwnerId { get; set; }

        [Display(Name = "City")]
        [Required]
        public Guid CityId { get; set; }

        /// <summary>
        /// The partition (i.e., account database) where we store this account's data.
        /// </summary>
        [Display(Name = "Partition")]
        [Required]
        public Guid PartitionId { get; set; }


        public List<SelectListItem> Cities { get; private set; } = new List<SelectListItem>();
        public List<SelectListItem> Partitions { get; private set; } = new List<SelectListItem>();
        public List<SelectListItem> Users { get; private set; } = new List<SelectListItem>();
        public List<SelectListItem> Buckets { get; set; } = new List<SelectListItem>();

        public string City { get; set; }
        public string State { get; set; }

        public string Zip { get; set; }

        [Display(Name = "AccountNumber")]
        public int AccountNumber { get; set; }

        public List<AccountViolationType> AccViolationType { get; set; } = new List<AccountViolationType>();


        public List<SelectListItem> CitationWorkflowItems { get; private set; } = new List<SelectListItem>();

        public Guid? AttachmentId { get; set; }

        public bool AllowPublicRegistration { get; set; }

    }
}
