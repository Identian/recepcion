using FluentValidation;
using MediatR;

namespace CasosDeUso.Validators
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        readonly IEnumerable<IValidator<TRequest>> _validator;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validator)
        {
            _validator = validator;
        }
        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            List<FluentValidation.Results.ValidationFailure> error = _validator
                .Select(e => e.Validate(request))
                .SelectMany(e => e.Errors)
                .Where(e => e != null)
                .ToList();
            if (error.Any())
            {
                throw new FluentValidation.ValidationException(error);
            }
            return next();
        }
    }
}
