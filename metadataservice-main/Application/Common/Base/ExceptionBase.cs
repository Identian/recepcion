using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Application.Common.Base
{
    /// <summary>
    /// Clase encargada de retornar el formato de la respuesta de acuerdo a la excepción que se produzca se puede personalizar totalmente el tipo de respuesta
    //  Remplazando la clase "ProblemDetails" por la clase que se requiera usar, para estandarizar las respuestas del servicio, así sea que se presente un error.
    /// </summary>
    public class ExceptionBase
    {

        readonly Dictionary<int, string> _errorsType = new()
        {
            { StatusCodes.Status400BadRequest, "Bad Request" },
            { StatusCodes.Status500InternalServerError, "Internal Server Error"}
        };

        public Task Result(ExceptionContext exceptionContext, int status, string title, string detail)
        {
            ProblemDetails problem = new()
            {
                Status = status,
                Title = title,
                Detail = detail,
                Type = _errorsType.ContainsKey(status) ? _errorsType[status] : null
            };

            exceptionContext.Result = new ObjectResult(problem) { 
                StatusCode = status,
            };
            exceptionContext.ExceptionHandled = true;
            return Task.CompletedTask;  
        }
    }
}
