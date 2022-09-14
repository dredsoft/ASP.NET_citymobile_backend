using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Enums
{
    public enum CitationAuditEvent
    {
        [Description("Ticket Created")]
        NewCitation = 1,
        [Description("Status Changed")]
        StatusChange = 2,
        [Description("Assignment")]
        Assignment = 3,
        [Description("Details Updated")]
        DetailsUpdated = 4,
        [Description("Misc")]
        Other = 5,
        [Description("Citation Payment Success")]
        CitationSuccessPayment = 6,
        [Description("Citation Failure Payment")]
        CitationFailurePayment = 7,
        [Description("Citation Payment Refunded")]
        CitationRefundPayment = 8,
        [Description("Violation Quiz Completed")]
        QuizComplete = 9,


    }
}
