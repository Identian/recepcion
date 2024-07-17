using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;

namespace CapaDominio.Interfaces.IServices
{
    public interface IEmailManagement
    {
        Task<IRespuesta> ProcesarMensajesPendientes(IReceptorBase infoReceptorBase, ICuentaCorreo infoCorreoReceptor, ILogAzure logAzure);
    }
}