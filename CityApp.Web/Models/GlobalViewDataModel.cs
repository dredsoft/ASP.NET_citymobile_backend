using CityApp.Common.Models;
using CityApp.Data.Enums;
using CityApp.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    /// <summary>
    /// Holds common data for .cshtml files that don't have easy acces to view models
    /// (e.g., _Layout, partials).
    /// </summary>
    public class GlobalViewDataModel
    {
        public long AccountNumber { get; set; }
        public string AccountName { get; set; }

        public Guid OwnerUserId { get; set; }
        public AccountPermissions? Permissions { get; set; }
        public AccountFeatures Features { get; set; }
        public bool UserOwnsAnAccount { get; set; }
        public string TimeZone { get; set; }
        public List<ViolationTypeModel> ViolationTypes { get; set; }

    }
}
