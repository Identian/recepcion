using DTO.RegistrarEmailDto;
using MediatR;

namespace CasosDeUso.Inputs.RegistrarEmail
{
    public class RegistrarEmailInput : GuardarEmailDto, IRequest<string>
    {
    }
}
