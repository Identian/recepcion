using CapaDominio.Radicacion;

namespace CapaDominio.Interfaces.IServices.IServicesDocuments
{
    public interface IEnviarRepresentacionGraficaProcesor
    {
        Task<IRespuestaRadicacion> LoadEnviarRepGrafica(string token, string archivo, RepresentacionGrafica repGrafica);
    }
}