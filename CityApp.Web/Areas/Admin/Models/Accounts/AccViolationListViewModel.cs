using CityApp.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class AccViolationListViewModel : ListBase
    {

        #region Constants

        public static readonly string NameSortParam = "Name";
        public static readonly string CategorySortParam = "Category";
        public static readonly string TypeSortParam = "Type";
        public static readonly string HelpSortParam = "HelpUrl";
        public static readonly string DisabledSortParam = "Disabled";

        #endregion

        public string Searchstring { get; set; }

        #region Category
        [Display(Name = "Category")]
        [Required]
        public Guid CategoryId { get; set; }
        [Display(Name = "CategoryName")]
        public string CategoryName { get; set; }
        public List<SelectListItem> Categories { get; private set; } = new List<SelectListItem>();
        #endregion

        #region Type
        [Display(Name = "Type")]
        [Required]
        public Guid TypeId { get; set; }
        [Display(Name = "TypeName")]
        public string TypeName { get; set; }

        public List<SelectListItem> Types { get; private set; } = new List<SelectListItem>();
        #endregion

        public List<AccViolationListItem> Violation { get; set; } = new List<AccViolationListItem>();
        public Guid AccountId { get; set; }

        public string CityName { get; set; }
    }
}
