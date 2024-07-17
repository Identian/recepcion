using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CasosDeUso.Exceptions.Base
{
    public class ExceptionBase
    {
        readonly Dictionary<int, string> _errorCode = new()
        {
            { StatusCodes.Status500InternalServerError, "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"},
            { StatusCodes.Status501NotImplemented,      "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.2"},
            { StatusCodes.Status404NotFound,            "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4"},
            { StatusCodes.Status403Forbidden,           "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3"},
            { StatusCodes.Status400BadRequest,          "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"},
            { StatusCodes.Status204NoContent,           "https://datatracker.ietf.org/doc/html/rfc7231#section-6.3.5"},
            { StatusCodes.Status202Accepted,            "https://datatracker.ietf.org/doc/html/rfc7231#section-6.3.3"},
            { StatusCodes.Status201Created,             "https://datatracker.ietf.org/doc/html/rfc7231#section-6.3.2"},
            { StatusCodes.Status200OK,                  "https://datatracker.ietf.org/doc/html/rfc7231#section-6.3.1"},
            { StatusCodes.Status502BadGateway,          "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.3"},
        };

        public Task Result(ExceptionContext context, int? status, string title, string details)
        {
            ProblemDetails problem = new()
            {
                Status = status,
                Title = title,
                Type = _errorCode.TryGetValue(status!.Value, out string? value) ? value : "",
                Detail = details
            };

            context.Result = new ObjectResult(problem)
            {
                StatusCode = status,
            };
            context.ExceptionHandled = true;
            return Task.CompletedTask;
        }

    }
}
