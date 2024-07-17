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
    public class PaginateProvidersValidator : AbstractValidator<PaginateProvidersInputPort>
    {
        public PaginateProvidersValidator()
        {
            RuleFor(p => p.StartIndex)
                .NotNull()
                .WithMessage("Campo requerido.")
                .Custom((value, contex) =>
                {
                    if (value < 0)
                    {
                        contex.AddFailure("El valor debe ser mayor que 0");
                    }
                });
            RuleFor(p => p.EndIndex)
                .NotEmpty().WithMessage("Campo requerido.")
                .NotNull().WithMessage("Campo requerido.")
                .Custom((value, contex) =>
                {
                    if (value <= 0)
                    {
                        contex.AddFailure("El valor debe ser mayor que 0.");
                    }

                    if (value > 100)
                    {
                        contex.AddFailure("El valor maximo de resultados paginados es de 100.");
                    }

                });
            RuleFor(p => p.AplicationRoot)
                .NotEmpty().WithMessage("Campo requerido.")
                .NotNull().WithMessage("Campo requerido.")
                .Custom((value, contex) =>
                {
                    if (value <= 0)
                    {
                        contex.AddFailure("El valor debe ser mayor que cero.");
                    }
                });
            RuleFor(p => p.AplicationUser)
                .NotEmpty().WithMessage("Campo requerido.")
                .NotNull().WithMessage("Campo requerido.")
                .Custom((value, contex) =>
                {
                    if (value <= 0)
                    {
                        contex.AddFailure("El valor debe ser mayor que cero.");
                    }
                });
            RuleFor(p => p.IdEnterprise)
                .NotEmpty().WithMessage("Campo requerido.")
                .NotNull().WithMessage("Campo requerido.")
                .Custom((value, contex) =>
                {
                    if (value <= 0)
                    {
                        contex.AddFailure("El valor debe ser mayor que cero.");
                    }
                });

            RuleFor(p => p.BuscarProveedor)
                .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Campo requerido.")
                .When(p => !string.IsNullOrEmpty(p.BuscarProveedor))
                    .Matches("^[a-zA-Z0-9]*$")
                        .WithMessage("Ingresa el número de Nit o Razón social del proveedor.");

            RuleFor(p => p.OrderByDesc)
                .NotNull().WithMessage("Campo requerido.");
        }
    }
}
