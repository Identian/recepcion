using Application.UsesCases.Inputs;
using FluentValidation;

namespace Application.Validators
{
    public class LinkUsersProvidersValidator : AbstractValidator<LinkUserAndProvidersInputPort>
    {
        public LinkUsersProvidersValidator()
        {
            RuleFor(v => v.AplicationUser)
                .NotNull().WithMessage("Campo requerido.")
                .NotEmpty().WithMessage("Campo requerido.")
                .Custom((value, context) =>
                {
                    if (value <= 0)
                    {
                        context.AddFailure("El valor debe ser diferente de 0.");
                    }
                });
            RuleFor(v => v.IdReceptor)
               .NotNull().WithMessage("Campo requerido.")
               .NotEmpty().WithMessage("Campo requerido.")
               .Custom((value, context) =>
               {
                   if (value <= 0)
                   {
                       context.AddFailure("El valor debe ser diferente de 0.");
                   }
               });

            RuleFor(v => v.Providers)
                .Custom((value, context) =>
                {
                    if (value.Count <= 0)
                    {
                        context.AddFailure("No se suministraron datos de proveedores que asociar.");
                    }
                });


            RuleForEach(v => v.Providers)
                .SetValidator(new ProvidersStatusValidator());
        }
    }
}
