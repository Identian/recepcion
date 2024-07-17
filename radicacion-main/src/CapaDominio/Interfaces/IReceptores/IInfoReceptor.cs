using CapaDominio.RequestReceptor;

namespace CapaDominio.Interfaces.IReceptores
{
    public interface IInfoReceptor
    {
        CuentaCorreo cb { get; set; }
        RequestReceptor.Receptor r { get; set; }
    }
}