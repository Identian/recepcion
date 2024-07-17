using Application.Common.Base;
using Application.Common.Interfaces;
using Domain.Exceptions;
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
    /// Filtro de clase de GeneralException donde se llame esta Exception sera gestionada por el filtro
    /// </summary>
    public class GeneralExceptionHandler : ExceptionBase, IExceptionHandler
    {
        public Task Handle(ExceptionContext context)
        {
            GeneralException? generalException = context.Exception as GeneralException;
            return Result(context, StatusCodes.Status400BadRequest, generalException!.Message ?? "", generalException.Details ?? "");
        }
    }
}
