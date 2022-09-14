using CityApp.Common.Models;
using System.Collections.Generic;

namespace CityApp.Api.Test.Models
{
    public class CityAppResponse<T>
    {
        public bool Success { get; set; } = true;

        public string Message { get; set; }

        public List<Error> Errors { get; set; } = new List<Error>();

        public T Data { get; set; }
    }
}
