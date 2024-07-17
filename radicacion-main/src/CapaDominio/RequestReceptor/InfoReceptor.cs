using CapaDominio.Interfaces.IReceptores;

namespace CapaDominio.RequestReceptor
{
    public class InfoReceptor : IInfoReceptor
    {
        public CuentaCorreo cb { get; set; } = new CuentaCorreo();
        public Receptor r { get; set; } = new Receptor();
    }
}
