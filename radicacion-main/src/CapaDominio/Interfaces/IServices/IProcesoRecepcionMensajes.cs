using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using MailKit.Net.Imap;

namespace CapaDominio.Interfaces.IServices
{
    public interface IProcesoRecepcionMensajes
    {
        IRespuesta AbrirBandeja(string carpeta, string usuario, ImapClient clienteImap, ILogAzure log);
        Task<IRespuesta> OpenFoldersTFHKA(string usuario, ImapClient clienteImap, ILogAzure log);
        string CrearCarpeta(ImapClient imapC, string carpeta);
        IRespuesta LeerUltimoMensaje(object bandejaEntrada, ILogAzure log, int elem = -1);
        IRespuesta MarcarMensajeLeido(object ultimoUid, object bandejaMensaje, ILogAzure log);
        IRespuesta MoverMensaje(object ultimoUid, object bandejaOrigen, object bandejaDestino, ILogAzure log);
        Task<IRespuesta> RadicarMensaje(string usuario, object ultimoUid, object ultimoMensaje, object bandejaDescargados, object bandejaErroneos, object pCarpetaDeEmailActual, IReceptorBase infoContribuyente, ILogAzure log, bool esBandejaEntrada);
    }
}
