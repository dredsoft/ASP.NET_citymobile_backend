using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CityApp.Common.Serialization
{
    /// <summary>
    /// Serialize using the camel case contract resolver.
    /// </summary>
    public static class JsonNet
    {
        private static readonly JsonSerializerSettings _camelCase = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private static readonly JsonSerializerSettings _camelCaseIgnoreRefLoop = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        private static readonly JsonSerializerSettings _pascalCase = new JsonSerializerSettings
        {
        };

        private static readonly JsonSerializerSettings _pascalCaseIgnoreRefLoop = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static string Serialize(object value, bool camelCase = true, bool ignoreReferenceLoop = false, bool prettyPrint = false)
        {
            var formatting = prettyPrint ? Formatting.Indented : Formatting.None;

            if (camelCase)
            {
                return ignoreReferenceLoop
                    ? JsonConvert.SerializeObject(value, formatting, _camelCaseIgnoreRefLoop)
                    : JsonConvert.SerializeObject(value, formatting, _camelCase);
            }

            return ignoreReferenceLoop
                ? JsonConvert.SerializeObject(value, formatting, _pascalCaseIgnoreRefLoop)
                : JsonConvert.SerializeObject(value, formatting, _pascalCase);
        }


        private static readonly JsonSerializerSettings _camelCaseEscapeHtml = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            StringEscapeHandling = StringEscapeHandling.EscapeHtml
        };

        private static readonly JsonSerializerSettings _camelCaseEscapeHtmlIgnoreRefLoop = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        private static readonly JsonSerializerSettings _pascalCaseEscapeHtml = new JsonSerializerSettings
        {
            StringEscapeHandling = StringEscapeHandling.EscapeHtml
        };

        private static readonly JsonSerializerSettings _pascalCaseEscapeHtmlIgnoreRefLoop = new JsonSerializerSettings
        {
            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static string SerializeEscapeHtml(object value, bool camelCase = true, bool ignoreReferenceLoop = false)
        {
            if (camelCase)
            {
                return ignoreReferenceLoop
                    ? JsonConvert.SerializeObject(value, _camelCaseEscapeHtmlIgnoreRefLoop)
                    : JsonConvert.SerializeObject(value, _camelCaseEscapeHtml);
            }

            return ignoreReferenceLoop
                ? JsonConvert.SerializeObject(value, _pascalCaseEscapeHtml)
                : JsonConvert.SerializeObject(value, _pascalCaseEscapeHtmlIgnoreRefLoop);
        }

        public static T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
