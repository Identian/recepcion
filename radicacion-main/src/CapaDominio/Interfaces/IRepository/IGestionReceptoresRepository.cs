using CapaDominio.RequestReceptor;
using CapaDominio.Response;

namespace CapaDominio.Interfaces.IRepository
{
    public interface IGestionReceptoresRepository
    {
        Task<IRespuesta> GestionarReceptor(InfoReceptor infoReceptor);
    }
}
