using Application.DTO;
using FluentValidation;

namespace Application.Validators
{
    public class ProvidersStatusValidator : AbstractValidator<RequestProvidersAndStatusDto>
    {
        public ProvidersStatusValidator() {
            RuleFor(v => v.IdProvider)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Campo requerido.")
                .NotEmpty().WithMessage("Campo requerido.")
                .Must(BeAValidInteger)
                    .WithMessage("El campo Numero debe ser un entero válido.")
                    .WithErrorCode("InvalidIntegerValue")
                .Custom((value, context) =>
                {
                    if (value <= 0)
                    {
                        context.AddFailure("El valor debe ser diferente de 0.");
                    }
                });
            RuleFor(v => v.Estatus)
               .NotNull().WithMessage("Campo requerido.");

        }

        private bool BeAValidInteger(int value)
        {
            return int.TryParse(value.ToString(), out _);
        }
    }
}
