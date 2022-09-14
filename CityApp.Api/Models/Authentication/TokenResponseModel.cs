using CityApp.Data.Enums;
using System;

namespace CityApp.Api.Models
{
    public class TokenResponseModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public SystemPermissions Permission { get; set; }
        public string ProfileImageUrl { get; set; }

        public string Token { get; set; }
        public long Expires { get; set; }
    }
}
