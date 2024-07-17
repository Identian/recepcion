using CapaDominio.Response;
using DTO.Authenticate;
using MediatR;

namespace CasosDeUso.Inputs.GuardarTenant
{
    public class SaveTenantInput : SaveTenantAuthorizationDto, IRequest<IRespuestaApi>
    {
    }
}
