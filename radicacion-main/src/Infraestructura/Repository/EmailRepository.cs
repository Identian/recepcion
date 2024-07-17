using CapaDominio.Enums.Logs;
using CapaDominio.Errors;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using MailKit.Net.Imap;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace Infraestructura.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly ILogAzure _logAzure;
        private readonly IEmailConectar _emailConectar;
        public EmailRepository(ILogAzure logAzure, IEmailConectar emailConectar)
        {
            _logAzure = logAzure;
            _emailConectar = emailConectar;
        }

        public async Task<string> ConsultarEmail(ICuentaCorreo param)
        {
            IRespuesta respuesta = new Respuesta();
            try
            {
                Stopwatch timeT = new();
                timeT.Start();

                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "EmailPing Inicio " + param.Usuario, LevelMsn.Info, timeT.ElapsedMilliseconds);

                respuesta = await _emailConectar.Conectar(param, _logAzure);
                //remove el cliente
                respuesta.ValorObject.RemoveAt(0);
                _logAzure.SaveLog(respuesta.Codigo.ToString(), "Verificar acceso al correo", "", MethodBase.GetCurrentMethod()!.Name, respuesta.Descripcion!, param.Usuario!, ref timeT);
            }
            catch (Exception ext)
            {
                respuesta.Codigo = 500;
                respuesta.Descripcion = ErrorsCodes._500A;
                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "EmailPing Exception " + _logAzure.ConvertToJson(ext), LevelMsn.Info);
                Stopwatch timeT = new();
                timeT.Start();
                _logAzure.SaveLog(respuesta.Codigo.ToString(), "Verficiar acceso al correo", "", MethodBase.GetCurrentMethod()!.Name, "Error en consulta de correo electrónico", param.Usuario!, ref timeT);
            }
            return JsonConvert.SerializeObject(new { respuesta.Codigo, Mensaje = respuesta.Descripcion });
        }

    }
}
