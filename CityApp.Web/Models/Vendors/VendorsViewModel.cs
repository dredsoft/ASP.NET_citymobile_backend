using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Vendors
{
    public class VendorsViewModel
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        [MaxLength(150)]
        [Required]
        [Display(Name ="Vendor Name")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Mobile Number")]        
        [MinLength(10)]
        [MaxLength(15)]
        public string MobilePhone { get; set; }

        [Display(Name ="Office Number")]
        [MinLength(10)]
        [MaxLength(15)]
        public string OfficePhone { get; set; }

        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address 2")]
        public string Address2 { get; set; }

        public string City { get; set; }
        public string State { get; set; }

        public string Zip { get; set; }

        public bool Disabled { get; set; }

        public ViolationActions Actions { get; set; }

    }
}
