using CapaDominio.Receptor;

namespace CapaDominio.Interfaces.IRepository
{
    public interface IEmailInactivarRepository
    {
        Task<string> InactivarEmail(ConsultaEmail emailInactivar);
    }
}
