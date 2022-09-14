using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class CitationListViewModel
    {
        public List<CitationListModel> CitationList { get; set; }

        public List<AccountUserList> AccountList { get; set; }

    }
}
