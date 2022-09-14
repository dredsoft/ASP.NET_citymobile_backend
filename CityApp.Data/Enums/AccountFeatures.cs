using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CityApp.Data.Enums
{
    /// <summary>
    /// This class represent permission user can have for different sections of the applications.
    /// Enum flags can represetn more than one permission.  They are flexible and take less space to store. 
    /// Some referense that explain this concept.
    /// http://zduck.com/2013/using-enum-flags-to-model-user-roles-in-csharp-and-sql/
    /// http://www.winwire.com/flags-enumeration-roles-net-applications/
    /// https://stackoverflow.com/questions/8447/what-does-the-flags-enum-attribute-mean-in-c
    /// </summary>
    [Flags]
    public enum AccountFeatures
    {
        [Display(Name = "Info and Events")]
        Info = 1 << 0,
    }
}
