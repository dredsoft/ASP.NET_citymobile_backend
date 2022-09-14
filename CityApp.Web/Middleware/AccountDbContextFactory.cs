using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using CityApp.Data;
using CityApp.Services;
using System.Linq;
using CityApp.Common.Utilities;

namespace CityApp.Web.Middleware
{
    public class AccountDbContextFactory
    {
        private readonly CommonContext _commonContext;
        private readonly HttpContext _httpContext;

        public AccountDbContextFactory(CommonContext commonContext, IHttpContextAccessor context)
        {
            _commonContext = commonContext;
            _httpContext = context.HttpContext;
        }

        public AccountContext CreateAccountContext()
        {
            AccountContext accountContext = null;

            var accountNum = GetAccountNumberFromRoute();

            accountContext = GetAccountContext(accountNum);

            return accountContext;
        }

        private long? GetAccountNumberFromRoute()
        {
            var routeUrl = string.Empty;

            if (_httpContext.Request.Path.HasValue && _httpContext.Request.Path.Value.Split('/').Length > 1)
            {
                routeUrl = _httpContext.Request.Path.Value.Split('/')[1]?.Trim();
            }

            long acctNum;
            if (long.TryParse(routeUrl, out acctNum))
            {
                return acctNum;
            }

            return null;
        }

        public AccountContext GetAccountContext(long? accountNumber)
        {
            var accountCtx = new AccountContext(new DbContextOptions<AccountContext>());

            if (accountNumber != null)
            {
                //TODO: We have to be able to cache this info somehow.
                var commonAccount = _commonContext.CommonAccounts
                    .Include(ca => ca.Partition)
                    .Where(ca => ca.Number == accountNumber)
                    .AsNoTracking()
                    .SingleOrDefault();

                if (commonAccount != null)
                {
                    // Dispose the one we created at the top of this method.
                    accountCtx.Dispose();

                    // Create a new context using the partition's connection string.
                    accountCtx = ContextsUtility.CreateAccountContext(Cryptography.Decrypt(commonAccount.Partition.ConnectionString));
                }
            }

            return accountCtx;
        }
    }
}
