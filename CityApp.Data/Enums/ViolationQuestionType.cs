using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data.Enums
{
    public enum ViolationQuestionType
    {
        [Description("Text")]
        TextField = 1,
        [Description("Url")]
        UrlField,
        [Description("Email")]
        EmailField,
        [Description("Date Time")]
        DatetimeField,
        [Description("Date")]
        DateField,
        [Description("Bool")]
        BooleanField,
        [Description("Int")]
        ItegerField,
        [Description("Decimal")]
        FloatField,
        [Description("Single Choice")]
        SingleChoiceField,
        [Description("Multiple Choice")]
        MultiChoiceField,
        [Description("Multiple Checkbox")]
        MultiCheckboxField,
        [Description("Text Area")]
        TextArea
    }
}
