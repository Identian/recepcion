using CapaDominio.Auth;
using CapaDominio.Response;

namespace CapaDominio.Interfaces.IRepository
{
    public interface ISaveTenantRepository
    {
        Task<IRespuestaApi> SaveTenant(Authenticate authenticate);
    }
}
