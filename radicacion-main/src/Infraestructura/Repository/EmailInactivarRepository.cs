using CapaDominio.Enums.Logs;
using CapaDominio.Errors;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Receptor;
using CapaDominio.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection;

namespace Infraestructura.Repository
{
    public class EmailInactivarRepository : IEmailInactivarRepository
    {
        private readonly ILogAzure _logAzure;
        private readonly IEmailConectar _emailConectar;
        public EmailInactivarRepository(ILogAzure logAzure, IEmailConectar emailConectar)
        {
            _logAzure = logAzure;
            _emailConectar = emailConectar;
        }

        public async Task<string> InactivarEmail(ConsultaEmail emailInactivar)
        {
            IRespuestaApi respuesta = new RespuestaApi
            {
                Codigo = 500,
                Mensaje = ErrorsCodes._500A
            };

            Stopwatch timeT = new();

            try
            {
                respuesta = await _emailConectar.InactivarCorreoReceptor(emailInactivar.NumeroIdentificacion!, emailInactivar.TipoIdentificacion!, _logAzure);
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 999;
                respuesta.Mensaje = "Error general";
                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", _logAzure.ConvertToJson(ex), LevelMsn.Info, 0);
                return JsonConvert.SerializeObject(respuesta);
            }
            _logAzure.SaveLog(respuesta.Codigo.ToString(), "Inactivar correo de receptor", emailInactivar.NumeroIdentificacion!, MethodBase.GetCurrentMethod()!.Name, respuesta.Mensaje, "", ref timeT);

            return JsonConvert.SerializeObject(new { respuesta.Codigo, respuesta.Mensaje });
        }
    }
}
