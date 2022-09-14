using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Accounts
{
    public class AccountInfoViewModel
    {
        public CommonAccount Account { get; set; }
        public List<AccountEvent> Events { get; set; } = new List<AccountEvent>();
    }

    public class AccountEvent
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreateUtc { get; set; }
    }
}
