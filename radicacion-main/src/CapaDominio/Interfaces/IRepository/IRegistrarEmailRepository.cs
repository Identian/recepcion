using CapaDominio.RequestReceptor;

namespace CapaDominio.Interfaces.IRepository
{
    public interface IRegistrarEmailRepository
    {
        Task<string> Registrar(CuentaCorreoGuardar cuentaCorreo);
    }
}
