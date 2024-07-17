using Application.UsesCases.Inputs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
    public class EnviarMetadataEmisorValidator : AbstractValidator<RequestEnviarMetadataEmisorInputPort>
    {
        public EnviarMetadataEmisorValidator()
        {
            RuleFor(r => r.NumeroDocumento)
                .NotEmpty().WithMessage("Campo requerido")
                .NotNull().WithMessage("Campo requerido")
                .MinimumLength(1).WithMessage("Longitud minima del documento es de 1 numeros o letras");
            
            RuleFor(r => r.Metadata)
                .Must(x => x.Count > 0)
                .WithMessage("Metadatos requeridos, no se enviaron datos que asociar a la factura.");

            RuleForEach(r => r.Metadata)
                .SetValidator(new MetadataListValidator());

            RuleFor(r => r.TipoDocumento)
                    .NotEmpty().WithMessage("Campo requerido")
                    .NotNull().WithMessage("Campo requerido")
                    .Matches(@"^[0-9]+$").WithMessage("El campo solo acepta numeros ejemplo 01, 91, 92")
                    .Matches(@"^(01|91|92)$").WithMessage("El campo solo acepta los siguientes números 01 (Factura), 91 (Nota crédito), 92 (Nota débito)");
        }
    }
}
