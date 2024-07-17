using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using MimeKit;

namespace CapaDominio.Interfaces.IServices
{
    public interface IProcesoRecepcionDocumentoZip
    {
        Task<IRespuesta> RadicarZip(MimeMessage mensaje, IReceptorBase infoContribuyente, ILogAzure log, string usuario);
    }
}