using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class CommonUserAccountModel
    {
        public Guid UserId { get; set; }
        public Guid AccountId { get; set; }
        public long AccountNumber { get; set; }
        public AccountPermissions Permissions { get; set; }
        public AccountFeatures Features { get; set; }
        public DateTime? ExpirationUtc { get; set; }
        public bool Disabled { get; set; }

        public string Name { get; set; }
       
    }
}
