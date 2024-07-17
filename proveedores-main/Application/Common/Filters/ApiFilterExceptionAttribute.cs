using Application.Common.Base;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Filters
{
    /// <summary>
    /// Esta clase se encarga de filtrar las diferentes implementaciones de casos de exception
    /// Ejemplo: GeneralException, ValidationException, de acuerdo a la exception que se quiera representar o validar
    /// Esta debe usar IExceptionHandler de lo contrario el filtro o midleware no lo podra ejecutar.
    /// </summary>
    public class ApiFilterExceptionAttribute : ExceptionFilterAttribute
    {
        readonly IDictionary<Type, IExceptionHandler> _exceptionHandlers;

        public ApiFilterExceptionAttribute(IDictionary<Type, IExceptionHandler> exceptionHandlers)
        {
            _exceptionHandlers = exceptionHandlers;
        }

        public override void OnException(ExceptionContext context)
        {
            Type exceptionType = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(exceptionType))
            {
                _exceptionHandlers[exceptionType].Handle(context);
            }
            else
            {
                //Si el tipo de exception no registra envia una por defecto
                new ExceptionBase().Result(context, StatusCodes.Status400BadRequest, "Error en la petición intentelo de nuevo.", "Error general");
            }
            base.OnException(context);
        }
    }
}
