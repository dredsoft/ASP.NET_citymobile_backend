using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Services.Models
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; } = true;

        public string Message { get; set; }

        public List<Error> Errors { get; set; } = new List<Error>();

        public T Data { get; set; }
    }
}
