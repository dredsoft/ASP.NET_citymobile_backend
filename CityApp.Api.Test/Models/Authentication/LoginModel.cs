using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Api.Test.Models.Authentication
{
    public class LoginModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string DeviceName { get; set; }

        public string DeviceType { get; set; }

        public string DeviceToken { get; set; }
    }
}
