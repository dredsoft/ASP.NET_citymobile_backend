using CityApp.Data.Enums;
using CityApp.Web.Models;
using cloudscribe.Web.Pagination;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models.Users
{
    public class UserListViewModel : ListBase
    {
        #region Constants

        public static readonly string EmailSortParam = "Email";
        public static readonly string FirstNameSortParam = "FirstName";
        public static readonly string LastNameSortParam = "LastName";
        public static readonly string PermissionSortParam = "Permission";


        #endregion


        public List<UserListItem> UsersList { get; set; }

        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string Permission { get; set; }

        public List<SelectListItem> systemPermission { get; private set; } = new List<SelectListItem>();
    }
}
