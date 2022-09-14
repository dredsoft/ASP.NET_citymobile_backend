using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class AccountUserAssociationModel
    {
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
    }
   
}
