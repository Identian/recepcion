using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;

namespace CapaDominio.Interfaces.IServices
{
    public interface IProcesoAuthentication
    {
        Task<IRespuesta> GuardarTenantID(int id_receptor, string email, string tenantID, ILogAzure log);
        Task<IRespuesta> VerificarAccesos(int id_receptor, string email, ILogAzure log);
    }
}