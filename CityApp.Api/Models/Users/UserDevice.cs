using CityApp.Data.Models;
using CityApp.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Models
{
    public class UserDevice
    {
        public string Message { get; set; }
        public List<UserDeviceInfo> Device { get; private set; } = new List<UserDeviceInfo>();
    }
}
