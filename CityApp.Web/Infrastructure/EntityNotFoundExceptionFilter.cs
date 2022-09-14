using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CityApp.Web.Infrastructure
{
    /// <summary>
    /// Catch EntityNotFoundExceptions, which are thrown when doing a database lookup and not finding
    /// the record. Return a 404 to the caller.
    /// </summary>
    public class EntityNotFoundExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var entityNotFoundEx = context.Exception as EntityNotFoundException;
            if (entityNotFoundEx == null)
            {
                // Not the type of exception we're looking for. Carry on.
                return;
            }

            // Return a 404 to the client.
            context.ExceptionHandled = true;
            context.Result = new NotFoundResult();
        }
    }
}
