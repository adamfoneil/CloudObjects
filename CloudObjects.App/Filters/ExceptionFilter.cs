using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CloudObjects.App.Filters
{
    /// <summary>
    /// for any unhandled exception here, we want to simply return the raw error message and prevent a 500 error
    /// </summary>
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new BadRequestObjectResult(context.Exception.Message);
        }
    }
}
