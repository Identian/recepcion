using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using MimeKit;

namespace CapaDominio.Interfaces.IServices
{
    public interface IRadicacion
    {
        Task<IRespuesta> RadicarArchivoAsync(MimeMessage mensaje, IReceptorBase infoContribuyente, ILogAzure log, string usuario);
    }
}