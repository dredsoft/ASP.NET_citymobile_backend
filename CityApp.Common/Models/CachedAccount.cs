using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Models
{
    public class CachedAccount
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public long Number { get; set; }
        public bool Suspended { get; set; }
        public Guid OwnerUserId { get; set; }

        public string PartitionName { get; set; }
        public string CityTimeZone { get; set; }
        public AccountFeatures Features { get; set; }
        public ViolationTypeModel[] ViolationTypes { get; set; }
        public CachedAccountSettings Settings { get; set; }

        public override string ToString() => $"{Name} ({Number}; Partition={PartitionName})";
    }
}
