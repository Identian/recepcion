using CasosDeUso.Inputs.InactivarCorreo;
using FluentValidation;

namespace CasosDeUso.ValidatorDto
{
    public class InactivarCorreoValidator : AbstractValidator<InactivarEmailInput>
    {
        public InactivarCorreoValidator()
        {
            RuleFor(p => p.NIT)
               .NotNull().WithMessage("El número de identificación es obligatorio.!")
               .NotEmpty().WithMessage("El número de identificación es obligatorio.!")
               .MaximumLength(12).WithMessage("Por favor ingrese un numero de identificación valido!")
               .Custom((id, contex) =>
               {
                   if (!long.TryParse(id, out long resultado))
                   {
                       contex.AddFailure("El número de identificación no es valido.");
                   }
               });
            RuleFor(p => p.TipoIdentificacion)
                .MaximumLength(2).WithMessage("El tipo de identificación no es valido, intente nuevamente!")
                .MinimumLength(2).WithMessage("El tipo de identificación no es valido, intente nuevamente!")
                .NotNull().WithMessage("El tipo de identificación es obligatorio!")
                .NotEmpty().WithMessage("El tipo de identificación es obligatorio!");
        }
    }
}
