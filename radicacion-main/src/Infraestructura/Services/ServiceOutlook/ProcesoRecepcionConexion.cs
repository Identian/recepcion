using CapaDominio.Enums.Logs;
using CapaDominio.Interfaces.IProcesos.Conexion;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using MailKit.Net.Imap;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Infraestructura.Services.ServiceOutlook
{
    public class ProcesoRecepcionConexion : IProcesoRecepcionConexion
    {
        private readonly IConfiguration _configuration;
        private readonly ImapClient _imapClient;
        readonly IRespuesta _respuesta;

        public ProcesoRecepcionConexion(IConfiguration configuration, IRespuesta respuesta)
        {
            _configuration = configuration;
            _respuesta = respuesta;
            _imapClient = new();
        }

        public IRespuesta Conectar(string servidor, int puerto, bool usarSsl, string usuario, string clave, ILogAzure log)
        {
            IRespuesta result = _respuesta;
            result.SetMetodo($"{nameof(ProcesoRecepcionConexion)} . {nameof(Conectar)}");
            _imapClient.Timeout = int.Parse(_configuration.GetSection("Imap:TimeOut").Value ?? "30000");
            try
            {
                _imapClient.Connect(servidor, puerto, usarSsl);
                try
                {
                    _imapClient.Authenticate(usuario, clave);
                    result.Resultado = true;
                }
                catch (Exception e)
                {
                    result.Codigo = 999;
                    result.Descripcion = "No se pudo autenticar el usuario de la cuenta de correo '" + usuario + "' con las credenciales proporcionadas";
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Codigo = 999;
                result.Descripcion = "No se pudo establecer conexión con el servidor de la cuenta de correo '" + usuario + "'";
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
                return result;
            }
            if (result.Resultado == false)
            {
                result.Detalles = "Servidor=" + servidor + ";Puerto=" + puerto.ToString() + ";UsarSSL=" + usarSsl.ToString() + ";Usuario=" + usuario + ";Clave=" + "*".PadLeft(clave.Length, '*');
            }

            result.DetallesAdicionales = _respuesta.GetInfoCompleta(omitirSaltosDeLinea: true);
            return result;
        }
    }
}
