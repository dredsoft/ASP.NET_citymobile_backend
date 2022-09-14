using System;

namespace CityApp.Common.Caching
{
    /// <summary>
    /// Cache keys used in the web application.
    /// </summary>
    public static class WebCacheKey
    {
        public static string CommonAccount(long accountNumber) => $"CommonAccount:{accountNumber}";
        public static string CommonAccounts = $"CommonAccounts";
        public static string CommonUserAccount(long accountNumber, Guid loggedInUserId) => $"CommonUserAccount:{loggedInUserId}:{accountNumber}";
        public static string LoggedInUser(Guid loggedInUserId) => $"LoggedInUser:{loggedInUserId}";
        public static string HealthCheck = "HealthCheck";

        public static string AccountViolations(Guid accountId) => $"AccountViolations:{accountId}";

        public static string CommonViolationTypes = $"CommonViolationTypes";
        public static string CommonViolationCategories = $"CommonViolationCategories";
        public static string CommonViolations = $"CommonViolations";
        public static string Violations = $"Violations";


    }
}
