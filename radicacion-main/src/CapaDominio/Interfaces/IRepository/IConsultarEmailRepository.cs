using CapaDominio.Receptor;

namespace CapaDominio.Interfaces.IRepository
{
    public interface IConsultarEmailRepository
    {
        Task<string> Consultar(ConsultaEmail consultaEmail);
    }
}
