using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace IdentityStart.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        var statusCode = exception switch
        {
            DbUpdateException or DbUpdateConcurrencyException => HttpStatusCode.Conflict,
            InvalidOperationException => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.BadRequest
        };

        var details = new ProblemDetails
        {
            Detail = exception.Message,
            Title = "An error occurred while processing your request.",
            Status = (int)statusCode
        };

        context.Result = new ObjectResult(details);
        context.HttpContext.Response.StatusCode = (int)statusCode;
        context.ExceptionHandled = true;
    }
}