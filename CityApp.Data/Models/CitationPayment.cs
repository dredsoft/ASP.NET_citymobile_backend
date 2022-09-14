using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Models
{
    public class CitationPayment : AccountEntity
    {
        public Guid CitationId { get; set; }

        public PaymentType Status { get; set; }

        public Citation Citation { get; set; }

        public double CitationFineAmount { get; set; }
         
        public double ProcessingFee { get; set; }

        public int ChargeAmount { get; set; }

        public string ChargeId { get; set; }

    }
}
