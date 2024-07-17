using CasosDeUso.Inputs.ConsultarEmail;
using FluentValidation;

namespace CasosDeUso.ValidatorDto
{
    public class ConsultaEmailValidator : AbstractValidator<ConsultarEmailInput>
    {
        public ConsultaEmailValidator()
        {

            RuleFor(p => p.numeroIdentificacion)
                .NotNull().WithMessage("El número de identificación es obligatorio!")
                .NotEmpty().WithMessage("El número de identificación es obligatorio!")
                .MaximumLength(20).WithMessage("La longitud del número de Nit no es válida!")
                .Custom((id, contex) =>
                {
                    if (!long.TryParse(id, out long resultado))
                    {
                        contex.AddFailure("Por favor ingrese un número de identificación válido!");
                    }
                });
            RuleFor(p => p.tipoIdentificacion)
                .NotNull().WithMessage("Tipo de identificación es obligatoria.")
                .NotEmpty().WithMessage("Tipo de identificación es obligatoria.")
                .MaximumLength(2).WithMessage("El tipo de identificación no es válido, intente nuevamente!")
                .MinimumLength(2).WithMessage("El tipo de identificación no es válido, intente nuevamente!");
        }
    }
}
