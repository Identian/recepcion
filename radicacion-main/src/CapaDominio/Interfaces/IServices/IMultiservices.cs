using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Radicacion;
using CapaDominio.Response;

namespace CapaDominio.Interfaces.IServices
{
    /// <summary>
    /// Interface encargada de el envio del archivo XML, PDF y anxo si contiene el correo.
    /// </summary>
    public interface IMultiservices
    {
        Task<IRespuesta> Run(ArchivoYReceptor archivoYReceptor, List<int> lRecErrores, ILogAzure logAzure);
    }
}