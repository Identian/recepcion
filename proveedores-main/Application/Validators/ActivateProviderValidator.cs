using Application.UsesCases.Inputs;
using FluentValidation;

namespace Application.Validators
{
    public class ActivateProviderValidator : AbstractValidator<ActivateProviderRequestInputPort>
    {
        public ActivateProviderValidator()
        {
            RuleFor(a => a.IdReceptor)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Campo requerido.")
                .NotNull().WithMessage("Campo requerido.")
                .Must(BeAValidInteger)
                    .WithMessage("El campo IdReceptor debe ser un entero válido.")
                    .WithErrorCode("InvalidIntegerValue")
                .Custom((values, contex) =>
                {
                    if (values <= 0)
                    {
                        contex.AddFailure("El identificador del receptor no es valido.");
                    }
                });
            RuleFor(a => a.IdProveedor)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Campo requerido.")
                .NotNull().WithMessage("Campo requerido.")
                .Must(BeAValidInteger)
                    .WithMessage("El campo IdProveedor debe ser un entero válido.")
                    .WithErrorCode("InvalidIntegerValue")
                .Custom((values, contex) =>
                {
                    if (values <= 0)
                    {
                        contex.AddFailure("El identificador del proveedor no es válido.");
                    }
                });
        }

        private bool BeAValidInteger(int value)
        {
            return int.TryParse(value.ToString(), out _);
        }
    }
}
