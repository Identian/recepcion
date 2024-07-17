using CapaDominio.Enums.Logs;
using CapaDominio.Enums.TipoAutenticacion;
using CapaDominio.Errors;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Interfaces.ServiceOutlook;
using CapaDominio.RequestReceptor;
using CapaDominio.Response;
using MailKit.Net.Imap;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Infraestructura.Services.GestionEmail
{
    public class EmailConectar : IEmailConectar
    {
        readonly IRespuesta _respuesta;
        readonly IOutlook _outlook;
        IRespuestaApi _respuestaApi;
        readonly IProcesoRecepcionMensajes _procesoRecepcionMensajes;
        readonly IProcesoGestionEmail _procesoGestionEmail;
        private IRespuestaApiConsultar _respuestaApiConsultar;
        private readonly IConfiguration _configuration;

        public EmailConectar(IRespuesta respuesta,
                             IOutlook outlook,
                             IRespuestaApi respuestaApi,
                             IProcesoRecepcionMensajes procesoRecepcionMensajes,
                             IProcesoGestionEmail procesoGestionEmail,
                             IRespuestaApiConsultar respuestaApiConsultar,
                             IConfiguration configuration)
        {
            _respuesta = respuesta;
            _outlook = outlook;
            _respuestaApi = respuestaApi;
            _procesoRecepcionMensajes = procesoRecepcionMensajes;
            _procesoGestionEmail = procesoGestionEmail;
            _respuestaApiConsultar = respuestaApiConsultar;
            _configuration = configuration;
        }
        public async Task<IRespuesta> Conectar(ICuentaCorreo cuentaCorreo1, ILogAzure log)
        {
            ImapClient clienteImap = new()
            {
                Timeout = int.Parse(_configuration.GetSection("Imap:TimeOut").Value ?? "30000")
            };

            _respuesta.SetMetodo($"{nameof(EmailManagement)} . {nameof(Conectar)}");

            CuentaCorreo? cuentaCorreo = cuentaCorreo1 as CuentaCorreo;

            try
            {
                try
                {
                    switch (cuentaCorreo!.TipoAutenticacion)
                    {
                        case TipoAutenticacion.AUTENTICACION_BASICA:
                            log.WriteComment(MethodBase.GetCurrentMethod()!.Name + "Autenticacion Básica IMAP", cuentaCorreo.Usuario!, LevelMsn.Info, 0);
                            clienteImap.CheckCertificateRevocation = false;
                            clienteImap.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                            await clienteImap.ConnectAsync(cuentaCorreo.Servidor, cuentaCorreo.Puerto, cuentaCorreo.UsarSSL);
                            await clienteImap.AuthenticateAsync(cuentaCorreo.Usuario, cuentaCorreo.Clave);
                            break;
                        case TipoAutenticacion.MICROSOFT_OAUTH_APLICACION:
                        case TipoAutenticacion.MICROSOFT_OAUTH_CODIGO:
                            log.WriteComment(MethodBase.GetCurrentMethod()!.Name + "Autenticacion OAUTH IMAP " + cuentaCorreo.TipoAutenticacion.ToString(), cuentaCorreo.Usuario!, LevelMsn.Info, 0);

                            try
                            {
                                string refreshToken = cuentaCorreo.RefreshToken!;
                                string token = cuentaCorreo.AccessToken!;
                                string tenant_id = cuentaCorreo.TenantID!;

                                RefrescarToken(cuentaCorreo.TipoAutenticacion, cuentaCorreo.Usuario!, ref refreshToken, ref token, ref tenant_id, log);

                                cuentaCorreo.RefreshToken = refreshToken;
                                cuentaCorreo.AccessToken = token;
                                cuentaCorreo.TenantID = tenant_id;
                                clienteImap.CheckCertificateRevocation = false;
                                clienteImap.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                                SaslMechanismOAuth2 Token = GetAccessToken(cuentaCorreo.Usuario!, cuentaCorreo.AccessToken);
                                await clienteImap.ConnectAsync("outlook.office365.com", 993, true);
                                await clienteImap.AuthenticateAsync(Token);
                            }
                            catch (AuthenticationException ex)
                            {
                                _respuesta.Resultado = false;
                                _respuesta.Codigo = 401;
                                _respuesta.Descripcion = string.Format(ErrorsCodes._401, cuentaCorreo.Usuario);
                                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + "Error refrescando Tokens OAUTH .Exception", log.ConvertToJson(new { Type = ex.GetType().FullName, ex.Message, ex.StackTrace }), LevelMsn.Info, 0);
                                return _respuesta;

                            }
                            catch (Exception ex)
                            {
                                _respuesta.Resultado = false;
                                _respuesta.Codigo = 410;
                                _respuesta.Descripcion = "Problemas conectando con el correo. Revise si se ejecutaron los permisos en AZURE";
                                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + "Error refrescando Tokens OAUTH .Exception", log.ConvertToJson(new { Type = ex.GetType().FullName, ex.Message, ex.StackTrace }), LevelMsn.Info, 0);
                                return _respuesta;
                            }
                            break;
                    }
                    _respuesta.Resultado = true;
                    _respuesta.Codigo = 200;
                    _respuesta.Descripcion = string.Format(ErrorsCodes._200, cuentaCorreo.Usuario);
                    string clave = cuentaCorreo.Clave ?? "*****";
                    _respuesta.Detalles = "Servidor=" + cuentaCorreo.Servidor + ";Puerto=" + cuentaCorreo.Puerto + ";UsarSSL=" + cuentaCorreo.UsarSSL + ";Usuario=" + cuentaCorreo.Usuario + ";Clave=" + "*".PadLeft(clave.Length, '*');
                    //Retorno el cliente Imap
                    _respuesta.ValorObject.Add(clienteImap);
                    return _respuesta;

                }
                catch (AuthenticationException ex)
                {
                    _respuesta.Resultado = false;
                    _respuesta.Codigo = 401;
                    _respuesta.Descripcion = string.Format(ErrorsCodes._401, cuentaCorreo!.Usuario);
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name + "Conectar con buzon .Exception", log.ConvertToJson(new { Type = ex.GetType().FullName, ex.Message, ex.StackTrace }), LevelMsn.Info, 0);
                    return _respuesta;
                }
                catch (Exception e)
                {
                    _respuesta.Resultado = false;
                    _respuesta.Codigo = 500;
                    _respuesta.Descripcion = string.Format(ErrorsCodes._500A, cuentaCorreo!.Usuario);
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name + "Conectar con buzon .Exception", log.ConvertToJson(new { Type = e.GetType().FullName, e.Message, e.StackTrace }), LevelMsn.Info, 0);
                    return _respuesta;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e is JsonSerializationException);
                Debug.WriteLine(e.StackTrace);
                _respuesta.Resultado = false;
                _respuesta.Codigo = 500;
                _respuesta.Descripcion = string.Format(ErrorsCodes._500, cuentaCorreo!.Usuario);
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + "Conectar con buzon  .Exception", log.ConvertToJson(new { Type = e.GetType().FullName, e.Message, e.StackTrace }), LevelMsn.Info, 0);
                return _respuesta;
            }
        }

        private void RefrescarToken(TipoAutenticacion tipo_autenticacion, string usuario, ref string refresh_Token, ref string access_Token, ref string tenant_id, ILogAzure log)
        {
            _outlook.SetTypeConecction(tipo_autenticacion, access_Token, refresh_Token, tenant_id, usuario);

            if (_outlook.Refresh_tokens(log).Result)
            {
                access_Token = _outlook.access_token;
                refresh_Token = _outlook.refresh_token;
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + " Guardar en base de datos Tokens OAUTH ", usuario, LevelMsn.Info, 0);
                _outlook.GuardarTokens(usuario, log);
            }
        }

        private static SaslMechanismOAuth2 GetAccessToken(string Username, string access_token)
        {
            return new SaslMechanismOAuth2(Username, access_token);
        }

        public async Task<IRespuestaApi> RegistrarActualizarEmail(CuentaCorreoGuardar DatosCorreo, ILogAzure log, ImapClient imapClient)
        {
            try
            {
                bool resultCreationFolder = true;
                resultCreationFolder |= _procesoRecepcionMensajes.AbrirBandeja(DatosCorreo.BandejaEntrada!, DatosCorreo.Usuario!, imapClient, log).Resultado;
                resultCreationFolder |= _procesoRecepcionMensajes.AbrirBandeja(DatosCorreo.BandejaDescargados!, DatosCorreo.Usuario!, imapClient, log).Resultado;
                resultCreationFolder |= _procesoRecepcionMensajes.AbrirBandeja(DatosCorreo.BandejaErroneos!, DatosCorreo.Usuario!, imapClient, log).Resultado;

                if (!resultCreationFolder)
                {
                    _respuestaApi.Codigo = 500;
                    _respuestaApi.Mensaje = string.Format(ErrorsCodes._500F, DatosCorreo.Usuario);
                }
                _respuestaApi = await _procesoGestionEmail.RegistrarActualizarEmail(DatosCorreo, log);
            }
            catch (Exception e)
            {
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Info, 0);
                _respuestaApi.Codigo = 500;
                _respuestaApi.Mensaje = ErrorsCodes._500A;
                return _respuestaApi;
            }
            log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Resultado " + _respuestaApi.Mensaje, LevelMsn.Info, 0);
            return _respuestaApi;
        }


        public async Task<IRespuestaApi> InactivarCorreoReceptor(string Nit, string TipoIdentificacion, ILogAzure log)
        {
            try
            {
                _respuestaApi = await _procesoGestionEmail.InactivarCorreoReceptor(Nit, TipoIdentificacion, log);
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Resultado " + _respuestaApi.Mensaje, LevelMsn.Info, 0);
                return _respuestaApi;
            }
            catch (Exception e)
            {
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Info, 0);
                _respuestaApi.Codigo = 500;
                _respuestaApi.Mensaje = ErrorsCodes._500A;
                return _respuestaApi;
            }
        }

        public async Task<IRespuestaApiConsultar> ConsultarEmail(string numeroIdentificacion, string tipoIdentificacion, ILogAzure log)
        {
            try
            {
                _respuestaApiConsultar = await _procesoGestionEmail.ConsultarEmail(numeroIdentificacion, tipoIdentificacion, log);
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Resultado " + _respuestaApiConsultar.Mensaje, LevelMsn.Info, 0);
                return _respuestaApiConsultar;
            }
            catch (Exception e)
            {
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Info, 0);
                _respuestaApiConsultar.Codigo = 500;
                _respuestaApiConsultar.Mensaje = ErrorsCodes._500A;
                return _respuestaApiConsultar;
            }
        }

        /// <summary>
        /// Verificación certificados SSL 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        static bool MySslCertificateValidationCallback(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            try
            {
                if (sslPolicyErrors == SslPolicyErrors.None)
                {
                    return true;
                }

                // Note: MailKit will always pass the host name string as the `sender` argument.
                string host = (string)sender;

                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNotAvailable) != 0)
                {
                    // This means that the remote certificate is unavailable. Notify the user and return false.
                    Console.WriteLine("The SSL certificate was not available for {0}", host);
                    return false;
                }

                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
                {
                    // This means that the server's SSL certificate did not match the host name that we are trying to connect to.
                    X509Certificate2? certificate2 = certificate as X509Certificate2;
                    string? cn = certificate2 != null ? certificate2.GetNameInfo(X509NameType.SimpleName, false) : certificate!.Subject;

                    Console.WriteLine("The Common Name for the SSL certificate did not match {0}. Instead, it was {1}.", host, cn);
                    return false;
                }


                foreach (X509ChainElement element in chain!.ChainElements.Where(s => s.ChainElementStatus.Length > 0))
                {
                    foreach (X509ChainStatus error in element.ChainElementStatus)
                    {
                        Console.WriteLine("\t\u2022 {0}", error.StatusInformation);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message} ///////  {ex.StackTrace}");
                return false;
            }
            return false;
        }
    }
}
