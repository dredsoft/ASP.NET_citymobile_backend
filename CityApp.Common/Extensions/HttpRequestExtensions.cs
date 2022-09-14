using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Common.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null || request.Headers == null)
            {
                return false;
            }

            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
