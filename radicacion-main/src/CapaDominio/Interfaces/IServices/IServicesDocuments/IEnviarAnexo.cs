using CapaDominio.Radicacion;

namespace CapaDominio.Interfaces.IServices.IServicesDocuments
{
    /// <summary>
    /// Envio de archivos adjuntos en el .zipdel correo
    /// </summary>
    public interface IEnviarAnexo
    {
        Task<IRespuestaRadicacion> LoadEnviarAnexo(string token, ArchivoYReceptor archivoYReceptor);
    }
}