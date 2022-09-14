using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.AccountSettings
{
    public class AccountUserListItem
    {

        public string FullName { get; set; }

        public string Email { get; set; }

        public AccountPermissions Permissions { get; set; }


        public SystemPermissions UserType { get; set; }

        public bool Disabled { get; set; }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }


        
    }
}
