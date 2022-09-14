using CityApp.Data.Enums;
using CityApp.Data.Migrations.Account;
using CityApp.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Areas.Admin.Models
{
    public class AdminCitationListViewModel : CityApp.Web.Models.Citations.CitationsListViewModel
    {

        #region Filters

        [DisplayName("Account")]
        public Guid? AccountId { get; set; }

        public bool? HasBalance { get; set; }

        #endregion

        #region Dropdown List values

        public List<SelectListItem> Accounts { get; set; } = new List<SelectListItem>();

        #endregion

        #region search results


        #endregion

    }
}
