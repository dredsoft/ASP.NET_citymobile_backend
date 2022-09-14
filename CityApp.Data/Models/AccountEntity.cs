using CityApp.Data.Models;
using CityApp.Data.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class AccountEntity : Entity, IHasAccount
    {
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        public AccountUser UpdateUser { get; set; }
        public AccountUser CreateUser { get; set; }
    }
}
