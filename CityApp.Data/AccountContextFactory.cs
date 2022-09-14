using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CityApp.Data
{
    /// <summary>
    /// Mystery solved.  This class is used for migrations.
    /// By the way, the syntax to create a migration is "dotnet ef migrations add [name] -c CommonContext"
    /// More info: http://benjii.me/2016/05/dotnet-ef-migrations-for-asp-net-core/
    /// </summary>
    public class AccountContextFactory : IDbContextFactory<AccountContext>
    {
        public AccountContext Create(DbContextFactoryOptions options)
        {
            var builder = new DbContextOptionsBuilder<AccountContext>();
            builder.UseSqlServer("Server=.;Database=MT_Accounts01;Trusted_Connection=True;");
            return new AccountContext(builder.Options);
        }
    }
}