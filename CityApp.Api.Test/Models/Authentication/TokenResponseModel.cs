using CityApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Api.Test.Models.Authentication
{
    public class TokenResponseModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public SystemPermissions Permission { get; set; }

        public string Token { get; set; }
        public long Expires { get; set; }
    }
}
