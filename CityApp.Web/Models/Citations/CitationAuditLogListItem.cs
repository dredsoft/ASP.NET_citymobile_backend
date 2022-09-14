using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models
{
    public class CitationAuditLogListItem
    {
        public CitationAuditEvent Event { get; set; }


        public string Events { get; set; }
        public string Comment { get; set; }

        public DateTime CreatedDate { get; set; }

        //public string CreateUserId { get; set; }
        public string CreateUser { get; set; }

        public DateTime Date { get; set; }
       

    }
}
