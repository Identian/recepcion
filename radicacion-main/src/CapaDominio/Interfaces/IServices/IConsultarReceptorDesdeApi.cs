using CapaDominio.Interfaces.IReceptores;

namespace CapaDominio.Interfaces.IServices
{
    public interface IConsultarReceptorDesdeApi
    {
        Task<IReceptor> ConsultaReceptor(int idReceptor);
    }
}