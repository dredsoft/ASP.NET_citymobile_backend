using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class Vendor : AccountEntity
    {
        [MaxLength(150)]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [MaxLength(250)]
        public string Address1 { get; set; }

        [MaxLength(250)]
        public string Address2 { get; set; }

        [MaxLength(200)]
        public string City { get; set; }

        [MaxLength(50)]
        public string State { get; set; }

        [DataType(DataType.PostalCode)]
        public string Zip { get; set; }

        public bool Disabled { get; set; }

        [MinLength(10)]
        [MaxLength(15)]
        public string OfficePhone { get; set; }

        [MinLength(10)]
        [MaxLength(15)]
        public string MobilePhone { get; set; }

        public ViolationActions Actions { get; set; }

    }
}
