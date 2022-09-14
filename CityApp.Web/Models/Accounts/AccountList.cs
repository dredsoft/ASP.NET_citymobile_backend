using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class AccountList
    {
        public List<AccountListItem> Accounts { get; set; } = new List<AccountListItem>();
    }

    public class AccountListItem
    {
        public Guid Id { get; set; }
        public long Number { get; set; }
        public string Name { get; set; }
        public string CityName { get; set; }
    }
}
