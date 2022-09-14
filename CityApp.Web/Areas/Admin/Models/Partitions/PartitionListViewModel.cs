using CityApp.Web.Models;
using cloudscribe.Web.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Areas.Admin.Models.Partitions
{
    public class PartitionListViewModel:ListBase
    {
        #region Constants

        public static readonly string NameSortParm = "Name";
        public static readonly string OccupantsSortParm = "Occupants";       

        #endregion

        public List<PartitionListItem> Partition { get; set; } = new List<PartitionListItem>();
        
    }
}
