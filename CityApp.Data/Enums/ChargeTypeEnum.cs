using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Enums
{
    public enum ChargeTypeEnum
    {
        [Description("Citation Payment")]
        PayCitation,
        [Description("Quiz Payment")]
        WarningQuiz,
    }
}
