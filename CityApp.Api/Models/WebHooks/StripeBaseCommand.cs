using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models.WebHooks
{
    public class StripeBaseCommand
    {
        public List<string> Logs { get; private set; } = new List<string>();
    }
}
