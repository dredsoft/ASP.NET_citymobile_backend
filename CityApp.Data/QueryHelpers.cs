using CityApp.Data.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityApp.Data
{
    public static class QueryHelpers
    {
        public static IQueryable<T> ForAccount<T>(this IQueryable<T> query, Guid accountId)
            where T : IHasAccount
        {
            return query.Where(q => q.AccountId == accountId);
        }
    }
}
