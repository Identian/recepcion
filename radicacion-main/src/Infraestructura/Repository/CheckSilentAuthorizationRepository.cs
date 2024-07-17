using CapaDominio.Auth;
using CapaDominio.Enums.Logs;
using CapaDominio.Errors;
using CapaDominio.Interfaces.IHelpers;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using Infraestructura.Logs;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Infraestructura.Repository
{
    public class CheckSilentAuthorizationRepository : ICheckSilentAuthorizationRepository
    {
        private readonly IProcesoAuthentication _procesoAuthentication;
        private readonly IConfiguration _configuration;
        private readonly IObjectConversion<State> _objectConversion;

        public CheckSilentAuthorizationRepository(IProcesoAuthentication procesoAuthentication, IConfiguration configuration, IObjectConversion<State> objectConversion)
        {
            _procesoAuthentication = procesoAuthentication;
            _configuration = configuration;
            _objectConversion = objectConversion;
        }

        public async Task<IRespuestaApi> CheckSilent(CheckSilentAuthorization checkSilent)
        {
            IRespuestaApi respuesta = new RespuestaApi()
            {
                Codigo = 500,
                Mensaje = "Error desconocido. "
            };

            Stopwatch timeT = new();

            timeT.Start();

            ILogAzure logAzure = new LogAzure(_configuration, "0");

            try
            {
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Verificando entrada de datos", LevelMsn.Info, 0);
                if (checkSilent.State!.Equals(null))
                {
                    respuesta.Codigo = 400;
                    respuesta.Mensaje = ErrorsCodes._400;
                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "No hay datos de entrada", LevelMsn.Info, 0);
                    return respuesta;
                }
                string cleanState = Uri.UnescapeDataString(checkSilent.State!).TrimEnd('#');
                byte[] byteState = Convert.FromBase64String(cleanState);
                string stateFinal = Encoding.UTF8.GetString(byteState);

                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Decodificando " + stateFinal, LevelMsn.Info, 0);
                State StateObj = _objectConversion.FromJson(stateFinal);

                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de verificar Accesos " + StateObj.Email, LevelMsn.Info, 0);

                IRespuesta resultado = await _procesoAuthentication.VerificarAccesos(StateObj.Id_receptor, StateObj.Email!, logAzure);

                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Despues de verificar Accesos " + StateObj.Email, LevelMsn.Info, 0);

                respuesta.Codigo = resultado.Codigo;
                respuesta.Mensaje = resultado.Descripcion!;

                logAzure.SaveLog("200", "Concluye autorizacion Silente", "31", "GestionCorreo", respuesta.Mensaje + " -- " + respuesta.Codigo, StateObj.Email!, ref timeT);
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Mensaje += ex.Message;
                logAzure.SaveLog("500", "Concluye autorizacion Silente", "31", "GestionCorreo", respuesta.Mensaje, "Excepcion CheckSilentAuthorization", ref timeT);
                return respuesta;
            }
        }
    }
}
