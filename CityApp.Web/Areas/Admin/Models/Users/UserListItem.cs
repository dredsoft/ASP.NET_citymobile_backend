using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Areas.Admin.Models
{
    public class UserListItem
    {
        public string FullName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public string Permission { get; set; }

        public string Account { get; set; }

        public string Partition { get; set; }

        public DateTime CreateUtc { get; set; }


        public Guid Id { get; set; }


    }
}




