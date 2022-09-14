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
    public enum ViolationRequiredFields
    {
        [Display(Name = "Vehicle Information")]
        VehicleInformation = 1 << 0,

        [Display(Name = "Video")]
        Video = 1 << 1,

        [Display(Name = "Photo")]
        Photo = 1 << 2,

        [Display(Name = "Video Audio")]
        VideoAudio = 1 << 3,
    }


}
