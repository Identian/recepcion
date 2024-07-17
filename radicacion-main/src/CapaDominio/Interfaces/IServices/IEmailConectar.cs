using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.RequestReceptor;
using CapaDominio.Response;
using MailKit.Net.Imap;

namespace CapaDominio.Interfaces.IServices
{
    public interface IEmailConectar
    {
        Task<IRespuesta> Conectar(ICuentaCorreo cuentaCorreo1, ILogAzure log);
        Task<IRespuestaApi> InactivarCorreoReceptor(string Nit, string TipoIdentificacion, ILogAzure log);
        Task<IRespuestaApiConsultar> ConsultarEmail(string numeroIdentificacion, string tipoIdentificacion, ILogAzure log);
        Task<IRespuestaApi> RegistrarActualizarEmail(CuentaCorreoGuardar DatosCorreo, ILogAzure log, ImapClient imapClient);
    }
}