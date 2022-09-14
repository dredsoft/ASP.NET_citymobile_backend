using CityApp.Common.Models;
using CityApp.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class AccountViewModel
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Owner")]
        public Guid OwnerId { get; set; }

        [Display(Name = "City")]
        [Required]
        public Guid CityId { get; set; }

        /// <summary>
        /// The partition (i.e., account database) where we store this account's data.
        /// </summary>
        [Display(Name = "Partition")]
        [Required]
        public Guid PartitionId { get; set; }

        [Display(Name = "Citation Counter")]
        [Required]
        public long CitationCounter { get; set; } = 1000000;


        public List<SelectListItem> Cities { get; private set; } = new List<SelectListItem>();
        public List<SelectListItem> Partitions { get; private set; } = new List<SelectListItem>();
        public List<SelectListItem> Users { get; private set; } = new List<SelectListItem>();
        public List<SelectListItem> Buckets { get; set; } = new List<SelectListItem>();

        [Display(Name = "CityName")]
        public string CityName { get; set; }

        [Display(Name = "PartitionName")]
        public string PartitionName { get; set; }

        [Display(Name = "AccountNumber")]
        public int AccountNumber { get; set; }

        public List<AccountViolationType> AccViolationType { get; set; } = new List<AccountViolationType>();

        [Display(Name = "S3 Bucket")]
        [MaxLength(100)]
        public string StorageBucketName { get; set; }

        [Display(Name = "Workflow")]
        [Required]
        public CitationWorkflow CitationWorkflow { get; set; }

        public AccountFeatures Features { get; set; }

        public List<SelectListItem> CitationWorkflowItems { get; private set; } = new List<SelectListItem>();

    }

    public class Select2PagedResult
    {
        public int Total { get; set; }
        public List<SelectListItem> Results { get; private set; } = new List<SelectListItem>();
    }
}
