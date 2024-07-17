using CasosDeUso.Exceptions.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CasosDeUso.Exceptions.Presentadores
{
    /// <summary>
    /// Esta clase presenta los errores que se puedan presentar en la aplicacion o donde se llame la Excepcion Personalizada
    /// GeneralException
    /// </summary>
    public class GeneralExceptionHandler : ExceptionBase, IExeptionHandler
    {
        public Task Handle(ExceptionContext context)
        {
            GeneralException? exception = context.Exception as GeneralException;
            return Result(context, StatusCodes.Status404NotFound, exception!.Message, exception.Details!);
        }
    }
}
