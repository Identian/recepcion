using CapaDominio.Response;
using DTO.Authenticate;
using MediatR;

namespace CasosDeUso.Inputs.CheckSilent
{
    public class CheckSilentAuthorizationInput : CheckSilentAuthorizationDto, IRequest<IRespuestaApi>
    {
    }
}
