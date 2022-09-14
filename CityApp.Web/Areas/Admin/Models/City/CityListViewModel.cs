using CityApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Areas.Admin.Models
{
    public class CityListViewModel : ListBase
    {
        #region Constants

        public static readonly string NameSortParam = "Name";
        public static readonly string StateSortParam = "State";

        #endregion

        public List<CityListItem> CityList { get; set; }

        public string Name { get; set; }
        public string State { get; set; }


    }
}
