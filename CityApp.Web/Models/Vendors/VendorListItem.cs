using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Vendors
{
    public class VendorListItem
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

       
        public string Name { get; set; }

       
        public string Email { get; set; }

       
        public string MobilePhone { get; set; }
    }
}
