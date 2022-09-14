using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Vendors
{
    public class AccountUserVendorListViewModel: ListBase
    {

        #region Constants

        public static readonly string FullNameSortParam = "Name";
        public static readonly string EmailSortParam = "Email";       
        public Guid Id { get; set; }

        #endregion

        public List<AccountUserVendorListItem> AccountUserVendorListItem { get; set; }

       
    }
}
