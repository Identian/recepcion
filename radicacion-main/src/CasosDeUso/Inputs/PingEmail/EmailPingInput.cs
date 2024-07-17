using DTO.EmailPing;
using MediatR;

namespace CasosDeUso.Inputs.PingEmail
{
    public class EmailPingInput : EmailPingDto, IRequest<string>
    {
    }
}
