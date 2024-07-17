using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Behaviours
{
    /// <summary>
    /// 
    /// MidleWare Filter Etc..
    /// 
    /// 
    /// Esta clase lo que hace es validar que el request que esta recibiendo un Controlador sea igual o de tipo igual a la respuesta
    /// Al usar el Patron Mediator tenemos un IRequest que en este caso se usan por los inputPorts y un IRequestHandler que es usado por los
    /// Interactors, estos request los validamos con FluentValidations antes de que se pueda ejecutar algo contra la base de datos
    /// Validar el codigo y comprobar el uso de cada uno de estos que se mensionan.
    /// 
    /// Luego de la validación si no existen errores sigue
    /// 
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationBehaviours<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehaviours(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var errores = _validators
                .Select(e => e.Validate(request))
                .SelectMany(e => e.Errors)
                .Where(e => e != null)
                .ToList();
            if (errores.Count != 0)
            {
                throw new ValidationException(errores);
            }
            return next();
        }
    }
}
