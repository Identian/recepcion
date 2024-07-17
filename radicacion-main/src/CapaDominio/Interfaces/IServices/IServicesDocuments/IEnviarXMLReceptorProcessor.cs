using CapaDominio.Radicacion;

namespace CapaDominio.Interfaces.IServices.IServicesDocuments
{
    public interface IEnviarXmlReceptorProcessor
    {
        Task<IEnviarXMLResponse> LoadEnviarXML(string token, string archivo);
    }
}