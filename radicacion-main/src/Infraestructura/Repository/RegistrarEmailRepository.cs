using CapaDominio.Enums.Logs;
using CapaDominio.Enums.TipoAutenticacion;
using CapaDominio.Errors;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Interfaces.IServices;
using CapaDominio.RequestReceptor;
using CapaDominio.Response;
using Infraestructura.Logs;
using MailKit.Net.Imap;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection;

namespace Infraestructura.Repository
{
    public class RegistrarEmailRepository : IRegistrarEmailRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailConectar _emailConectar;
        public RegistrarEmailRepository(IConfiguration configuration, IEmailConectar emailConectar)
        {
            _configuration = configuration;
            _emailConectar = emailConectar;
        }

        public async Task<string> Registrar(CuentaCorreoGuardar cuentaCorreo)
        {
            dynamic respuesta = new JObject();
            ImapClient clienteImap;
            LogAzure log = new(_configuration, "0");
            Stopwatch timeT = new();
            Stopwatch timeS = new();
            timeS.Start();
            try
            {
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "begin " + cuentaCorreo.Usuario, LevelMsn.Info, timeS.ElapsedMilliseconds);

                if (cuentaCorreo.TipoAutenticacion == TipoAutenticacion.MICROSOFT_OAUTH_CODIGO)
                {
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Autenticacion OAUTH " + cuentaCorreo.Usuario, LevelMsn.Info, timeS.ElapsedMilliseconds);
                    #region Validaciones de parámetros opcionales

                    //Si no vinieron los datos de las bandejas se toman por defecto las que están en el config.
                    if (string.IsNullOrWhiteSpace(cuentaCorreo.BandejaEntrada))
                    {
                        cuentaCorreo.BandejaEntrada = _configuration.GetSection("Bandejas:BandejaEntrada").Value ?? "";
                    }
                    if (string.IsNullOrWhiteSpace(cuentaCorreo.BandejaDescargados))
                    {
                        cuentaCorreo.BandejaDescargados = _configuration.GetSection("Bandejas:BandejaDescargados").Value ?? "";
                    }
                    if (string.IsNullOrWhiteSpace(cuentaCorreo.BandejaErroneos))
                    {
                        cuentaCorreo.BandejaErroneos = _configuration.GetSection("Bandejas:BandejaErroneos").Value ?? "";
                    }
                    if (string.IsNullOrWhiteSpace(cuentaCorreo.BandejaOtros))
                    {
                        cuentaCorreo.BandejaOtros = _configuration.GetSection("Bandejas:BandejaOtros").Value ?? "";
                    }
                    if (string.IsNullOrWhiteSpace(cuentaCorreo.Clave))
                    {
                        cuentaCorreo.Clave = "";
                    }
                    if (string.IsNullOrWhiteSpace(cuentaCorreo.Servidor))
                    {
                        cuentaCorreo.Servidor = "";
                    }
                    #endregion


                    #region Conectar OAUTH y guardar en Base de datos

                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de conectarse al correo " + cuentaCorreo.Usuario, LevelMsn.Info, timeS.ElapsedMilliseconds);

                    IRespuesta conexionCorreo = await _emailConectar.Conectar(cuentaCorreo, log);
                    clienteImap = (ImapClient)conexionCorreo.ValorObject[0];
                    
                    //Eliminar el ciente de la lista
                    conexionCorreo.ValorObject.RemoveAt(0);

                    if (!conexionCorreo.Resultado)
                    {
                        respuesta.Codigo = "400";
                        respuesta.Mensaje = ErrorsCodes._400R;
                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Error conectando al correo " + cuentaCorreo.Usuario, LevelMsn.Info, timeS.ElapsedMilliseconds);
                    }
                    else
                    {
                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de Guardar en Base de Datos " + cuentaCorreo.Usuario, LevelMsn.Info, timeS.ElapsedMilliseconds);
                        IRespuestaApi respuestaRegistro = await _emailConectar.RegistrarActualizarEmail(cuentaCorreo, log, clienteImap);
                        respuesta.Codigo = respuestaRegistro.Codigo;
                        respuesta.Mensaje = respuestaRegistro.Mensaje;
                    }
                    #endregion
                }

                else
                {
                    #region Validaciones de parámetros opcionales
                    //Si no vinieron los datos de las bandejas se toman por defecto las que están en el config.
                    if (string.IsNullOrWhiteSpace(cuentaCorreo.BandejaEntrada))
                    {
                        cuentaCorreo.BandejaEntrada = _configuration.GetSection("Bandejas:BandejaEntrada").Value ?? "";
                    }
                    if (string.IsNullOrWhiteSpace(cuentaCorreo.BandejaDescargados))
                    {
                        cuentaCorreo.BandejaDescargados = _configuration.GetSection("Bandejas:BandejaDescargados").Value ?? "";
                    }
                    if (string.IsNullOrWhiteSpace(cuentaCorreo.BandejaErroneos))
                    {
                        cuentaCorreo.BandejaErroneos = _configuration.GetSection("Bandejas:BandejaErroneos").Value ?? "";
                    }
                    if (string.IsNullOrWhiteSpace(cuentaCorreo.BandejaOtros))
                    {
                        cuentaCorreo.BandejaOtros = _configuration.GetSection("Bandejas:BandejaOtros").Value ?? "";
                    }

                    #endregion

                    #region Conectar IMAP y guardar en Base de datos

                    IRespuesta conexionCorreo = await _emailConectar.Conectar(cuentaCorreo, log);
                    clienteImap = (ImapClient)conexionCorreo.ValorObject[0];
                    
                    //remover el cliente
                    conexionCorreo.ValorObject.Remove(0);

                    if (!conexionCorreo.Resultado)
                    {
                        respuesta.Codigo = "400";
                        respuesta.Mensaje = ErrorsCodes._400R;
                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name + " No se pudo conectar al correo ", cuentaCorreo.Usuario!, LevelMsn.Info, timeS.ElapsedMilliseconds);
                    }
                    else
                    {
                        IRespuestaApi respuestaRegistro = await _emailConectar.RegistrarActualizarEmail(cuentaCorreo, log, clienteImap);
                        respuesta.Codigo = respuestaRegistro.Codigo;
                        respuesta.Mensaje = respuestaRegistro.Mensaje;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ErrorsCodes._500A;
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(ex), LevelMsn.Info, 0);
            }
            timeS.Stop();
            log.SaveLog(respuesta.Codigo.ToString(), "Registrar o actualizar email", "", MethodBase.GetCurrentMethod()!.Name, "", cuentaCorreo.Usuario, ref timeT);
            return JsonConvert.SerializeObject(new { respuesta.Codigo, respuesta.Mensaje });
        }
    }
}
