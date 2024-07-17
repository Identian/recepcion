using CapaDominio.Auth;
using CapaDominio.Response;

namespace CapaDominio.Interfaces.IRepository
{
    public interface ICheckSilentAuthorizationRepository
    {
        Task<IRespuestaApi> CheckSilent(CheckSilentAuthorization checkSilent);
    }
}
