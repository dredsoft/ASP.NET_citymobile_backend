using CityApp.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class ViolationCategoryListViewModel : ListBase
    {
        #region Constants

        public static readonly string NameSortParam = "Name";
        public static readonly string TypeSortParam = "Type";

        #endregion

        public string Searchstring { get; set; }

        [Display(Name = "Type")]
        [Required]
        public Guid TypeId { get; set; }

        public List<SelectListItem> Types { get; private set; } = new List<SelectListItem>();

        [Display(Name = "TypeName")]
        public string TypeName { get; set; }

        public List<ViolationCategoryListItem> Violations { get; set; } = new List<ViolationCategoryListItem>();
    }
}
