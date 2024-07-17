using DTO.ConsultarEmailDto;
using MediatR;

namespace CasosDeUso.Inputs.ConsultarEmail
{
    public class ConsultarEmailInput : ConsultarCorreoParamsDto, IRequest<string>
    {
    }
}
