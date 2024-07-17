using CapaDominio.Interfaces.IReceptores;

namespace CapaDominio.Interfaces.IRepository
{
    public interface IEmailRepository
    {
        Task<string> ConsultarEmail(ICuentaCorreo param);
    }
}
