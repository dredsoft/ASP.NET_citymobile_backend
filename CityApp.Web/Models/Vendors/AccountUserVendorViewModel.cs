using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Vendors
{
    public class AccountUserVendorViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public Guid Id { get; set; }
    }
}
