using CasosDeUso.Exceptions.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CasosDeUso.Exceptions.Filter
{
    public class ApiFilterExceptions : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, IExeptionHandler> _exceptionsHandler;

        public ApiFilterExceptions(IDictionary<Type, IExeptionHandler> _exceptionsHandler)
        {
            this._exceptionsHandler = _exceptionsHandler;
        }

        public override void OnException(ExceptionContext context)
        {
            Type ExeptionType = context.Exception.GetType();

            if (_exceptionsHandler.TryGetValue(ExeptionType, out IExeptionHandler value))
            {
                value.Handle(context);
            }
            else
            {
                new ExceptionBase().Result(context, StatusCodes.Status500InternalServerError, "Error en la petición validar nuevamente", "");
            }
            base.OnException(context);
        }
    }
}
