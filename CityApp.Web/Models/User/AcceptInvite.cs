using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.User
{
    public class AcceptInvite
    {
        public string AccountName { get; set; }

        public string InvitedEmail { get; set; }

        public Guid AccountId { get; set; }

        public int Permission { get; set; }   

        public string Name { get; set; }

        public SystemPermissions SystemPermission { get; set; }

        public Guid VendorId { get; set; }




    }
}
