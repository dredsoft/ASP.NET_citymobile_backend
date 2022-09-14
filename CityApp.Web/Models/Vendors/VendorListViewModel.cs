using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Vendors
{
    public class VendorListViewModel: ListBase
    {

        public List<VendorListItem> VendorList { get; set; }
    }
}
