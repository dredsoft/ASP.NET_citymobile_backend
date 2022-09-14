using CityApp.Web.Models;
using cloudscribe.Web.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class AccountListViewModel : ListBase
    {

        #region Constants

        public static readonly string NameSortParam = "Name";
        public static readonly string CitySortParam = "CityName";
        public static readonly string PartitionSortParam = "Partition";
        public static readonly string FullNameSortParam = "FullName";

        #endregion


        public string Searchstring { get; set; }

        public List<AccountListItem> Accounts { get; set; } = new List<AccountListItem>();
       
    }
}
