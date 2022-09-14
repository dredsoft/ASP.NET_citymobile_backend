using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class AccountCategoryIndexViewModel
    {
        public Guid AccountId { get; set; }
        public CommonAccount Account { get; set; }
        public List<ViolationCategory> Categories { get; set; }
    }
}
