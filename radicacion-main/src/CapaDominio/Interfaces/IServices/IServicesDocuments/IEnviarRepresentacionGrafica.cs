using CapaDominio.Radicacion;

namespace CapaDominio.Interfaces.IServices.IServicesDocuments
{
    /// <summary>
    /// Envio de representación grafica.
    /// </summary>
    public interface IEnviarRepresentacionGrafica
    {
        Task<IRespuestaRadicacion> LoadEnviarRepGrafica(string token, ArchivoYReceptor archivoYReceptor);
    }
}