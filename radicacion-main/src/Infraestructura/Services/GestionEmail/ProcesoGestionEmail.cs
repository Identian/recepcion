using CapaDominio.Enums.Logs;
using CapaDominio.Enums.TipoAutenticacion;
using CapaDominio.Errors;
using CapaDominio.Interfaces.IDB;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.RequestReceptor;
using CapaDominio.Response;
using Infraestructura.Services.ServiceOutlook;
using System.Reflection;

namespace Infraestructura.Services.GestionEmail
{
    public class ProcesoGestionEmail : IProcesoGestionEmail
    {
        private readonly IDataBase _database;
        private readonly IRespuesta _respuesta;
        private readonly IRespuestaApi _respuestaApi;
        private IRespuestaApiConsultar _respuestaApiConsultar;

        public ProcesoGestionEmail(IDataBase database, IRespuesta respuesta, IRespuestaApi respuestaApi, IRespuestaApiConsultar respuestaApiConsultar)
        {
            _database = database;
            _respuesta = respuesta;
            _respuestaApi = respuestaApi;
            _respuestaApiConsultar = respuestaApiConsultar;
        }

        public async Task<IRespuestaApi> RegistrarActualizarEmail(CuentaCorreoGuardar parametros, ILogAzure log)
        {
            IRespuesta result = _respuesta;
            result.SetMetodo($"{nameof(ProcesoRecepcionConexion)} . {nameof(RegistrarActualizarEmail)}");
            try
            {
                result = await _database.RegistrarCuentaCorreoReceptor(parametros, log);
                _respuestaApi.Codigo = result.Codigo;
                _respuestaApi.Mensaje = result.Descripcion!.ToString();
                return _respuestaApi;
            }
            catch (Exception e)
            {
                _respuestaApi.Codigo = 500;
                _respuestaApi.Mensaje = ErrorsCodes._500A;
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
                return _respuestaApi;
            }

        }


        public async Task<IRespuestaApi> InactivarCorreoReceptor(string Nit, string TipoIdentificacion, ILogAzure log)
        {
            IRespuesta result = _respuesta;
            result.SetMetodo($"{nameof(ProcesoRecepcionConexion)} . {nameof(InactivarCorreoReceptor)}");


            try
            {
                result = await _database.InactivarCorreoReceptor(Nit, TipoIdentificacion, log);
                _respuestaApi.Codigo = result.Codigo;
                _respuestaApi.Mensaje = result.Descripcion!;
            }
            catch (Exception e)
            {
                _respuestaApi.Codigo = 500;
                _respuestaApi.Mensaje = ErrorsCodes._500A;
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
            }

            return _respuestaApi;
        }

        public async Task<IRespuestaApiConsultar> ConsultarEmail(string numeroIdentificacion, string tipoIdentificacion, ILogAzure log)
        {
            try
            {
                _respuestaApiConsultar = await _database.ConsultarEmail(numeroIdentificacion, tipoIdentificacion, log);
            }
            catch (Exception e)
            {
                _respuestaApiConsultar.Codigo = 500;
                _respuestaApiConsultar.Mensaje = ErrorsCodes._500A;
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
            }
            return _respuestaApiConsultar;
        }

        /// <summary>
        /// Retorna el tipo de autenticación configurado para el correo y enterprise enviado
        /// </summary>
        /// <param name="id_receptor"></param>
        /// <param name="email"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<TipoAutenticacion> ConsultarTipoAutenticacion(int id_receptor, string email, ILogAzure log)
        {
            IRespuesta respuesta = await _database.ConsultarTipoAutenticacion(id_receptor, email, log);
            if (respuesta.Resultado)
            {
                bool result = Enum.TryParse(respuesta.ValorString, out TipoAutenticacion tipoAutenticacion);
                return result ? tipoAutenticacion : throw new Exception(respuesta.Descripcion);
            }
            else
            {
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".ThrowException", respuesta.Descripcion!, LevelMsn.Error, 0);
                throw new ArgumentException($"{respuesta.Descripcion!}");
            }
        }
    }
}