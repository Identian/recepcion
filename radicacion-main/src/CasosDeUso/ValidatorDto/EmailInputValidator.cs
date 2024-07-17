using CasosDeUso.Inputs.PingEmail;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CasosDeUso.ValidatorDto
{
    public class EmailInputValidator : AbstractValidator<EmailPingInput>
    {


        public EmailInputValidator()
        {
            RuleFor(p => p.Usuario)
                .NotEmpty().WithMessage("El correo electrónico es obligatorio!")
                .NotNull().WithMessage("El correo electrónico es obligatorio!")
                .Custom((email, contex) =>
                {
                    if (!IsValidEmail(email!))
                    {
                        contex.AddFailure("El correo electrónico proporcionado no es válido.");
                    }
                });
            RuleFor(p => p.Servidor)
                .NotEmpty().WithMessage("El nombre del servidor es obligatorio!")
                .NotNull().WithMessage("El nombre del servidor es obligatorio!")
                .MaximumLength(100).WithMessage("El nombre del servidor no es valido!")
                .MinimumLength(10).WithMessage("El nombre del servidor no es valido!");
            RuleFor(p => p.Puerto)
                .NotEmpty().WithMessage("El número del puerto es obligatorio!")
                .MinimumLength(3).WithMessage("Ingrese por favor un numero de puerto valido!")
                .MaximumLength(4).WithMessage("Ingrese por favor un numero de puerto valido!")
                .Custom((port, contex) =>
                {
                    if (!int.TryParse(port, out int res))
                    {
                        contex.AddFailure("El número del puerto no es válido.");
                    }
                });
            RuleFor(p => p.UsarSSL)
                .NotEmpty().WithMessage("Se debe especificar el uso de encriptación por SSL")
                .NotNull().WithMessage("Se debe especificar el uso de encriptación por SSL");
            RuleFor(p => p.Clave)
                .NotNull().WithMessage("La clave de acceso es obligatoria, intente de nuevo.")
                .NotEmpty().WithMessage("La clave de acceso es obligatoria, intente de nuevo. ")
                .MinimumLength(8).WithMessage("Se debe proporcionar una clave de acceso válida.");
        }

        private static bool IsValidEmail(string email)
        {
            string expresion = "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$";
            Match match = Regex.Match(email, expresion);
            return match.Success;
        }
    }
}
