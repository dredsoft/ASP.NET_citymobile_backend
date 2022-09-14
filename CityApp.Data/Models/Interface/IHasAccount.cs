using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models.Interface
{
    public interface IHasAccount
    {
        Guid AccountId { get; set; }

        Account Account { get; set; }
    }
}
