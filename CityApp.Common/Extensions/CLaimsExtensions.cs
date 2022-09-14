using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CityApp.Common.Constants;
using CityApp.Api.Constants;

namespace CityApp.Common.Extensions
{
    public static class ClaimsExtensions
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(ClaimsExtensions));

        /// <summary>
        /// Try to grab the logged in user's id from the claims ticket.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Guid? GetLoggedInUserId(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                // The user is not logged in.
                return null;
            }

            var firstAuthenticated = user.Identities.FirstOrDefault(u => u.IsAuthenticated);
            if (firstAuthenticated == null)
            {
                // No authenticated identities.
                return null;
            }

            var firstSid = firstAuthenticated.FindFirst(ClaimTypes.Sid);
            if (firstSid == null)
            {
                _logger.Error($"User had an authenticated Identity, but no ClaimTypes.Sid claim with the user id.");
                return null;
            }

            if (!Guid.TryParse(firstSid.Value, out Guid loggedInUserId))
            {
                _logger.Error($"User had an authenticated Identity, but unable to parse Id from ClaimTypes.Sid with value {firstSid.Value}");
                return null;
            }

            // Found it.
            return loggedInUserId;
        }

        /// <summary>
        /// Try to grab the logged in user's id from the claims ticket.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Guid? GetJWTLoggedInUserId(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                // The user is not logged in.
                return null;
            }

            var firstAuthenticated = user.Identities.FirstOrDefault(u => u.IsAuthenticated);
            if (firstAuthenticated == null)
            {
                // No authenticated identities.
                return null;
            }

            var firstSid = firstAuthenticated.Claims.Where(m => m.Type == JWTClaim.Sid).SingleOrDefault();
            if (firstSid == null)
            {
                _logger.Error($"User had an authenticated Identity, but no ClaimTypes.Sid claim with the user id.");
                return null;
            }


            // Found it.
            return Guid.Parse(firstSid.Value);
        }


        public static string GetEmail(this ClaimsPrincipal user)
        {
            if (user == null)
            {
                // The user is not logged in.
                return null;
            }

            var firstAuthenticated = user.Identities.FirstOrDefault(u => u.IsAuthenticated);
            if (firstAuthenticated == null)
            {
                // No authenticated identities.
                return null;
            }

            var email = firstAuthenticated.Claims.Where(m => m.Type == Constants.Security.Email).FirstOrDefault();

            if (email == null)
            {
                _logger.Error($"User had an authenticated Identity, but no ClaimTypes.Email claim with the user id.");
                return null;
            }

            // Found it.
            return email.Value;
        }

    }

}
