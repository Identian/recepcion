using CapaDominio.Radicacion;

namespace CapaDominio.Interfaces.IServices.IServicesDocuments
{
    public interface IEnviarXml
    {
        Task<IEnviarXMLResponse> LoadEnviarXML(string token, ArchivoYReceptor archivoYReceptor);
    }
}