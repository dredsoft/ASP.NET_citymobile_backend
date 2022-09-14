using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;

namespace CityApp.Web.Infrastructure
{
    public static class HtmlHelperExtensions
    {
        public static string ClassIfTrue(this IHtmlHelper helper, bool isTrue, string trueClassName, string falseClassName = null)
        {
            return isTrue ? trueClassName : falseClassName;
        }

        public static IHtmlContent ToJson(this IHtmlHelper helper, object obj)
        {
            var settings = new JsonSerializerSettings
            {
                //ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            settings.Converters.Add(new JavaScriptDateTimeConverter());
            return helper.Raw(JsonConvert.SerializeObject(obj, settings));
        }

        public static List<SelectListItem> ToSelectListIems(this Dictionary<long, string> items)
        {
            var result = new List<SelectListItem>();

            if (items != null && items.Any())
            {
                result.AddRange(items.Select(r => new SelectListItem
                {
                    Text = r.Value,
                    Value = r.Key.ToString()
                })
                 .ToList());
            }

            return result;
        }
    }

}
