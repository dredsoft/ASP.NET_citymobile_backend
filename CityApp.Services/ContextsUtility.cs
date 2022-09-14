using Microsoft.EntityFrameworkCore;
using CityApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Services
{
    public static class ContextsUtility
    {
        public static AccountContext CreateAccountContext(string connectionString)
        {
            var builder = new DbContextOptionsBuilder<AccountContext>();
            builder.UseSqlServer(connectionString);

            return new AccountContext(builder.Options);
        }
    }
}
