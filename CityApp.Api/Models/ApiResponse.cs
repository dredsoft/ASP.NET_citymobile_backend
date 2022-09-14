using CityApp.Common.Models;
using System.Collections.Generic;

namespace CityApp.Api.Models
{
    public class APIResponse<T>
    {
        public bool Success { get; set; } = true;

        public string Message { get; set; }

        public List<Error> Errors { get; set; } = new List<Error>();

        public T Data { get; set; }
    }
}
