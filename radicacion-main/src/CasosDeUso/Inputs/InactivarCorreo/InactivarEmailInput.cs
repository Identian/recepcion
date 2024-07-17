using DTO.ConsultarEmailDto;
using MediatR;

namespace CasosDeUso.Inputs.InactivarCorreo
{
    public class InactivarEmailInput : ConsultarCorreoDto, IRequest<string>
    {
    }
}
