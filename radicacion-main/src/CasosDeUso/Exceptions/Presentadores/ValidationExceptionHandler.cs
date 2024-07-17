using CasosDeUso.Exceptions.Base;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace CasosDeUso.Exceptions.Presentadores
{
    /// <summary>
    /// Esta clase hace referencia al presentador de las excepciones que se puedan presentar con FluentValidator
    /// </summary>
    public class ValidationExceptionHandler : ExceptionBase, IExeptionHandler
    {
        public Task Handle(ExceptionContext context)
        {
            ValidationException? exception = context.Exception as ValidationException;
            StringBuilder builder = new();
            foreach (FluentValidation.Results.ValidationFailure? i in exception!.Errors)
            {
                builder.AppendLine(string.Format("Propiedad: {0} Error: {1}", i.PropertyName, i.ErrorMessage));
            }
            return Result(context, StatusCodes.Status404NotFound, exception.Message, builder.ToString());
        }
    }
}
