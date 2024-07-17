using CasosDeUso.Inputs.RegistrarEmail;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CasosDeUso.ValidatorDto
{
    /// <summary>
    /// Validador del request de guardar correo 
    /// </summary>
    public partial class SaveEmailValidator : AbstractValidator<RegistrarEmailInput>
    {
        public SaveEmailValidator()
        {
            RuleFor(p => p.NIT)
                .MaximumLength(20).WithMessage("El número de Nit no es valido, intente nuevamente.")
                .NotEmpty().WithMessage("El número de Nit es obligatorio, intente nuevamente.")
                .NotNull().WithMessage("El número de Nit es obligatorio, intente nuevamente.")
                .Custom((nit, context) =>
                {
                    if (!long.TryParse(nit, out long valor))
                    {
                        context.AddFailure("El número de nit solo debe contener números!");
                    }
                });

            RuleFor(p => p.Servidor)
                .NotEmpty().WithMessage("El servidor es obligatorio, intente nuevamente!")
                .NotNull().WithMessage("El servidor es obligatorio, intente nuevamente!")
                .MaximumLength(100).WithMessage("El servidor es obligatorio, intente nuevamente!");
            RuleFor(p => p.Puerto)
                .NotEmpty().WithMessage("El número de puerto es obligatorio, intente nuevamente!.")
                .NotNull().WithMessage("El número de puerto es obligatorio, intente nuevamente!.")
                .MaximumLength(4).WithMessage("El número de puerto no es válido, por favor valide e intente de nuevo!")
                .Custom((port, context) =>
                {
                    if (!short.TryParse(port, out short valor))
                    {
                        context.AddFailure("El valor del puerto no es valido por favor validar!");
                    }
                });
            RuleFor(p => p.UsarSSL)
                .NotEmpty().WithMessage("Por favor indicar el uso de SSL, campo obligatorio")
                .NotNull().WithMessage("Por favor indicar el uso de SSL, campo obligatorio");
            RuleFor(p => p.Usuario)
                .NotNull().WithMessage("El correo electrónico es obligatorio.")
                .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                .Custom((email, contex) =>
                {
                    if (!IsValidEmail(email!))
                    {
                        contex.AddFailure("El correo electrónico proporcionado no es válido.");
                    }
                });
            RuleFor(p => p.TipoIdentificadorReceptor)
                .NotEmpty().WithMessage("El tipo de identificación es obligatorio.")
                .NotNull().WithMessage("El tipo de identificación es obligatorio.")
                .MinimumLength(2).WithMessage("Por favor ingresar una tipo de indentificación valida!")
                .MaximumLength(3).WithMessage("Por favor ingresar una tipo de indentificación valida!");
        }
        private static bool IsValidEmail(string email)
        {
            string expresion = "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$";
            Match match = Regex.Match(email, expresion);
            return match.Success;
        }
    }
}
