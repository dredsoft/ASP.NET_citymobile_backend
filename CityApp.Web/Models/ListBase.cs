using cloudscribe.Web.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class ListBase
    {
        public int PageSize { get; set; } = 10;
        public int Page { get; set; } = 1;
        public string SortOrder { get; set; }
        public string SortDirection { get; set; } = "DESC";
        public PaginationSettings Paging { get; set; } = new PaginationSettings();

        public string OppositeSortDirection
        {
            get
            {
                return SortDirection == "ASC" ? "DESC" : "ASC";
            }
        }

    }
}
