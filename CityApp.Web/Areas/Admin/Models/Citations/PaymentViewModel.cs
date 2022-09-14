using CityApp.Data.Enums;
using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Areas.Admin.Models
{
    public class PaymentViewModel
    {
        public Guid? Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid CitationId { get; set; }

        [Required]
        [Display(Name = "Charge type")]
        public PaymentType Status { get; set; }

        public Citation Citation { get; set; }

        [Required]
        [Display(Name = "Portion of total that represents the fine amount")]
        public double CitationFineAmount { get; set; }

        [Required]
        [Display(Name = "Processing fee")]
        public double ProcessingFee { get; set; }

        [Required]
        [Display(Name = "Total Amount being collected from Violator")]
        public int ChargeAmount { get; set; }

        [Display(Name = "Charge description")]
        public string ChargeId { get; set; }

    }
}
