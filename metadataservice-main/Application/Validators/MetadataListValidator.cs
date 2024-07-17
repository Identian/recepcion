using Application.DTO;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Validators
{
    public class MetadataListValidator: AbstractValidator<MetadataDto>
    {
        public MetadataListValidator()
        {
            RuleFor(m => m.Code)
                .NotEmpty().WithMessage("Campo requerido")
                .NotNull().WithMessage("Campo requerido");
            RuleFor(m => m.Value)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Campo requerido")
                .NotNull().WithMessage("Campo requerido");
               
        }
    }
}
