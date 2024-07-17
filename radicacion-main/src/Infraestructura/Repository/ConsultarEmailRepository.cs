using CapaDominio.Enums.Logs;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Receptor;
using CapaDominio.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Diagnostics;
using System.Reflection;

namespace Infraestructura.Repository
{
    public class ConsultarEmailRepository : IConsultarEmailRepository
    {
        private readonly IEmailConectar _emailConectar;
        private readonly ILogAzure _logAzure;

        public ConsultarEmailRepository(IEmailConectar emailConectar, ILogAzure logAzure)
        {
            _emailConectar = emailConectar;
            _logAzure = logAzure;
        }
        public async Task<string> Consultar(ConsultaEmail consultaEmail)
        {
            IRespuestaApiConsultar respuesta = new RespuestaApiConsultar
            {
                Codigo = 999,
                Mensaje = "Error desconocido"
            };
            Stopwatch timeT = new();
            try
            {
                respuesta = await _emailConectar.ConsultarEmail(consultaEmail.NumeroIdentificacion!, consultaEmail.TipoIdentificacion!, _logAzure);
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 999;
                respuesta.Mensaje = $"Error general";
                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", _logAzure.ConvertToJson(ex), LevelMsn.Info, 0);
                return JsonConvert.SerializeObject(respuesta);
            }
            string correo = respuesta.ListadoCorreos != null ? respuesta.ListadoCorreos[0].correo : "";
            _logAzure.SaveLog(respuesta.Codigo.ToString(), "Consultar datos email", consultaEmail.NumeroIdentificacion!, MethodBase.GetCurrentMethod()!.Name, respuesta.Mensaje, correo, ref timeT);
            return JsonConvert.SerializeObject(respuesta, new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter> { new StringEnumConverter { CamelCaseText = false } }
            });
        }
    }
}
