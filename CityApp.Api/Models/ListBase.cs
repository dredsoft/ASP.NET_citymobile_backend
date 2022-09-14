using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class ListBase
    {
        public int PageSize { get; set; } = 10;
        public int Page { get; set; } = 1;
        //public string SortOrder { get; set; }
        //public string SortDirection { get; set; } = "ASC";
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }

        //public string OppositeSortDirection
        //{
        //    get
        //    {
        //        return SortDirection == "ASC" ? "DESC" : "ASC";
        //    }
        //}
    }
}
