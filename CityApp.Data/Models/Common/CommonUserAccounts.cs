using CityApp.Data.Enums;
using System;

namespace CityApp.Data.Models
{
    public class CommonUserAccount : Entity
    {
        public CommonUser User { get; set; }
        public Guid? UserId { get; set; }

        public CommonAccount Account { get; set; }
        public Guid AccountId { get; set; }

        public AccountPermissions Permissions { get; set; }
        public DateTime? ExpirationUtc { get; set; }
        public bool Disabled { get; set; }

        
    }
}
