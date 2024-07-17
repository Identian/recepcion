using Application.UsesCases.Inputs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class EnviarMetadataValidator : AbstractValidator<RequestEnviarMetadataReceptorInputPort>
    {
        public EnviarMetadataValidator()
        {
            RuleFor(r => r.NitEmisor)
                .NotEmpty().WithMessage("Campo requerido")
                .NotNull().WithMessage("Campo requerido")
                .MinimumLength(6).WithMessage("Longitud no válida para NIT identificador de emisor")
                .Custom((value, contex) =>
                {
                    string expresion = @"^[a-zA-Z0-9]+$";
                    if (!Regex.IsMatch(value, expresion))
                    {
                        contex.AddFailure("El identificador del emisor debe contener números y/o letras.");
                    }
                });
            RuleFor(r => r.TipoIdentificacionEmisor)
                .NotEmpty().WithMessage("Campo requerido")
                .NotNull().WithMessage("Campo requerido")
                .MinimumLength(2).WithMessage("Longitud mínima del tipo de identificación es de 2 numeros")
                .Custom((value, contex) =>
                {
                    string expresion = "^(11|12|13|21|22|31|41|42|91)$";
                    if (!Regex.IsMatch(value, expresion))
                    {
                        contex.AddFailure("Tipo de Identificación No Soportada; solo se admite uno de los siguientes valores 11,12,13,21,22,31,41,42 y 91");
                    }
                });

            RuleFor(r => r.NumeroDocumento)
                .NotEmpty().WithMessage("Campo requerido")
                .NotNull().WithMessage("Campo requerido")
                .MinimumLength(1).WithMessage("Longitud minima del documento es de 1 numeros o letras");

            RuleFor(r => r.Metadata)
                .Must(x => x.Count > 0)
                .WithMessage("Metadatos requeridos, no se enviaron datos que asociar a la factura.");


            RuleForEach(r => r.Metadata)
                .SetValidator(new MetadataListValidator());

            RuleFor(r => r.ApplicationType)
                .InclusiveBetween(0, 1).WithMessage("El valor debe ser (0) o (1)");

            When(r => r.ApplicationType == 1, () => {
                RuleFor(r => r.TipoDocumento)
                    .NotEmpty().WithMessage("Campo requerido")
                    .NotNull().WithMessage("Campo requerido")
                    .Matches(@"^[0-9]+$").WithMessage("El campo solo acepta números ejemplo 01, 91, 92")
                    .Matches(@"^(01|91|92)$").WithMessage("El campo solo acepta los siguientes números 01 (Factura), 91 (Nota crédito), 92 (Nota débito)");
            });
        }
    }
}
