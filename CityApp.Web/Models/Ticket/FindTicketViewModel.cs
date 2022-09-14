using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Ticket
{
    public class FindTicketViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        public long AccountNumber { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public long CitationNumber { get; set; }

        [Required(ErrorMessage = "License Plate is Required")]
        [Display(Name = "License Plate")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "State is Required")]
        public string State { get; set; }
    }
}
