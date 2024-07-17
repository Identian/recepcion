using Application.Common.Base;
using Application.Common.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Presenters
{
    /// <summary>
    /// Clase que se usa dentro de los filtro, se invoca cada vez que ocurra un error de validación en los request 
    /// </summary>
    public class ValidationExceptionHandler : ExceptionBase, IExceptionHandler
    {
        public Task Handle(ExceptionContext context)
        {
            ValidationException? validation = context.Exception as ValidationException;
            StringBuilder builder = new();
            foreach (ValidationFailure error in validation!.Errors)
            {
                builder.Append(string.Format("Error: ({0}) Mensaje: ({1}) ", error.PropertyName, error.ErrorMessage));
            }
            string[] mensaje= validation.Message.Split(':');
            string detalle = builder.ToString().Replace("\r\n", "");
            return Result(context, StatusCodes.Status400BadRequest, mensaje[0], detalle);
        }
    }
}
