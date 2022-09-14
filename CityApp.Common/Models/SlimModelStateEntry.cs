using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Models
{
    public class SlimModelStateEntry
    {
        public SlimError[] Errors { get; set; }
    }

    public class SlimError
    {
        public string ErrorMessage { get; set; }
    }
}
