using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.AccountSettings
{
    public class AccountUserListViewModel: ListBase
    {
        #region Constants

        public static readonly string FullNameSortParam = "FullName";
        public static readonly string EmailNameSortParam = "Email";
        public static readonly string PermissionsSortParam = "Permissions";
        public static readonly string StatusSortParam = "Status";
        public static readonly string UserTypeSortParam = "UserType";
        public string Searchstring { get; set; }

        public string UserType { get; set; }

        public long AccountNum { get; set; }

        #endregion

        public List<AccountUserListItem> AccountUserList { get; set; }
    }
}
