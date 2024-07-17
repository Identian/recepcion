using CapaDominio.Radicacion;

namespace CapaDominio.Interfaces.IServices.IServicesDocuments
{
    public interface IEnviarANexoProcessor
    {
        Task<IRespuestaRadicacion> LoadEnviarAnexo(string token, string archivo, dynamic anexo);
    }
}