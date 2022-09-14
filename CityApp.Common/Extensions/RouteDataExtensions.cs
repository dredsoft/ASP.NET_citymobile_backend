using CityApp.Common.Utilities;
using Microsoft.AspNetCore.Routing;


namespace CityApp.Common.Extensions
{
    public static class RouteDataExtensions
    {
        public static long? GetAccountNumberFromRoute(this RouteData routeData)
        {
            Check.NotNull(routeData, nameof(routeData));

            var accountNumRouteValue = (string)routeData.Values["accountNum"];

            long accountNum;
            if (long.TryParse(accountNumRouteValue, out accountNum))
            {
                return accountNum;
            }

            return null;
        }
    }
}
