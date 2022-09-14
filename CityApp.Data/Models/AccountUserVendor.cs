using CityApp.Data.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class AccountUserVendor : Entity
    {
        public AccountUser AccountUser { get; set; }
        public Guid AccountUserId { get; set; }

        public Vendor Vendor { get; set; }
        public Guid VendorId { get; set; }


    }
}
