using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class AccountListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string CityName { get; set; }
        public string PartitionName { get; set; }
        public long Number { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
