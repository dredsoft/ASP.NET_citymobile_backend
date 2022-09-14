using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CityApp.Data.Models
{
    /// <summary>
    /// A CommonAccount has just enough information to find which account database an account lives in.
    /// Also, subscription information is maintained here.
    /// Create a migration: Add-Migration -Name {Your_Migration_Name} -Context CityApp.Data.CommonContext
    /// </summary>
    public class CommonAccount : Entity
    {
        /// <summary>
        /// Account number. Per FB2-187, new accounts are assigned MAX(Number) + a random number between 1 and 9.
        /// </summary>
        public long Number { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public Guid? OwnerUserId { get; set; }
        public CommonUser OwnerUser { get; set; }


        /// <summary>
        /// The partition (i.e., account database) where we store this account's data.
        /// </summary>
        public Guid PartitionId { get; set; }
        public Partition Partition { get; set; }

        /// <summary>
        /// 1-to-1, where CommonAccountSetting.Id = CommonAccount.Id.
        /// </summary>
        public CommonAccountSettings Settings { get; set; } = new CommonAccountSettings();

        public Guid CityId { get; set; }

        public City City { get; set; }

        [MaxLength(100)]
        public string StorageBucketName { get; set; }

        public CitationWorkflow CitationWorkflow { get; set; }

        /// <summary>
        /// If suspended, no acquisitions or dispositions can be committed to this account.
        /// </summary>
        public bool Suspended { get; set; }

        public AccountFeatures Features { get; set; }

        /// <summary>
        /// Account Owner no longer wants to see this account.
        /// </summary>
        public bool Archived { get; set; }


        [MaxLength(250)]
        public string Address1 { get; set; }


        [MaxLength(250)]
        public string Address2 { get; set; }


        [MinLength(10)]
        [MaxLength(15)]
        public string ContactNumber { get; set; }

        [MaxLength(200)]
        public string CityName { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [MaxLength(50)]
        public string Zip { get; set; }

        public Guid? AttachmentId { get; set; }

        [MaxLength(255)]
        public string ContactEmail { get; set; }


        public bool AllowPublicRegistration { get; set; }


        public List<CommonAccountViolationType> CommonAccountViolationTypes { get; set; } = new List<CommonAccountViolationType>();

    }
}
