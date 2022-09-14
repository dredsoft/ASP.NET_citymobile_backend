using CityApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class ViolationTypeListViewModel : ListBase
    {
        #region Constants

        public static readonly string NameSortParam = "Name";
        public static readonly string DescriptionSortParam = "Description";
        
        #endregion

        public string Searchstring { get; set; }

        public List<ViolationTypeListItem> Violations { get; set; } = new List<ViolationTypeListItem>();


    }
}
