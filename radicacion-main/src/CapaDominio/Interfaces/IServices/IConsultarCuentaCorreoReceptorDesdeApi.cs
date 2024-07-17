using CapaDominio.Interfaces.IReceptores;

namespace CapaDominio.Interfaces.IServices
{
    public interface IConsultarCuentaCorreoReceptorDesdeApi
    {
        Task<ICuentaCorreo> ConsultarCuentaCorreoReceptor(int idCuentaCorreoReceptor);
    }
}