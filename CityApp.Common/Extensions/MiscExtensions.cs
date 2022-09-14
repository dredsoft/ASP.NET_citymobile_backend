using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Extensions
{
    public static class MiscExtensions
    {
        public static double PenniesToDollarAmount(this int pennies)
        {
            var dollar = 0f;
            if(pennies > 0)
            {
                dollar = pennies / 100;
            }

            return dollar;
        }
    }
}
