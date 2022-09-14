using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Enums
{
    /// <summary>
    /// Warning, Do not change the name of these enums.  The mobile apps use the name.  Only change the Display Name
    /// </summary>
    [Flags]
    public enum ViolationActions
    {
        [Display(Name = "Towable")]
        Towable = 1 << 0,

        [Display(Name = "Booting")]
        Booting = 1 << 1,

    }

   
}
