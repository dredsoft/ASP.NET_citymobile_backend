using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Web.Models.Ticket
{
    public class TicketPaymentModel
    {
        public Guid CitationId { get; set; }
        public Guid AccountId { get; set; }

        public long AccountNumber { get; set; }
        public int CiationNumber { get; set; }

        public int AmountDue { get; set; }
        public string StripeToken { get; set; }
        public string StripeTokenType { get; set; }
        public string StripeEmail { get; set; }

    }
}
