using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CityApp.Api.Constants
{
    public static class JWTClaim
    {
        public static string Sid = JwtRegisteredClaimNames.Sid;
        public static string Sub = JwtRegisteredClaimNames.Sub;
        public static string Jti = JwtRegisteredClaimNames.Jti;
        public static string Iat = JwtRegisteredClaimNames.Iat;
        public static string SystemPermission = "SystemPermission";
    }
}
