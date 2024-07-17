using CapaDominio.Enums.Logs;
using CapaDominio.Interfaces.IAuth;
using CapaDominio.Interfaces.IHelpers;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.IServices.IServicesDocuments;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Radicacion;
using CapaDominio.Response;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;

namespace Infraestructura.Services.RadicacionDocumentos
{
    public class Multiservices : IMultiservices
    {
        private readonly ILoginService _loginService;
        private readonly IEnviarXml _enviarXml;
        private readonly IEnviarRepresentacionGrafica _enviarRepresentacion;
        private readonly IEnviarAnexo _enviarAnexo;
        private ILoginResponse? _login;


        public Multiservices(ILoginService loginService, IEnviarXml enviarXml, IEnviarRepresentacionGrafica enviarRepresentacion, IEnviarAnexo enviarAnexo)
        {
            _loginService = loginService;
            _enviarXml = enviarXml;
            _enviarRepresentacion = enviarRepresentacion;
            _enviarAnexo = enviarAnexo;
        }

        public async Task<IRespuesta> Run(ArchivoYReceptor archivoYReceptor, List<int> lRecErrores, ILogAzure logAzure)
        {
            IRespuesta respuesta = new Respuesta();
            Stopwatch timeT = new();
            timeT.Start();
            string archivo;
            string usuario;
            string numID;
            string msg;
            try
            {
                if (!string.IsNullOrEmpty(archivoYReceptor.TokenEnterprise) && !string.IsNullOrEmpty(archivoYReceptor.TokenPassword))
                {
                    archivo = archivoYReceptor.DocElectronicoRecibido!;
                    usuario = archivoYReceptor.usuario!;
                    numID = archivoYReceptor.NumeroIdentificacionReceptor!;
                    string token;

                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Radicando archivo {archivoYReceptor.NombreXml}", LevelMsn.Info, 0);

                    //*******Radicar archivo XML*******//
                    _login ??= _loginService.LoadLogin(archivoYReceptor.TokenEnterprise!, archivoYReceptor.TokenPassword!);
                    bool expirationToken = Convert.ToDateTime(_login.passwordExpiration) > DateTime.Now;
                    if (!expirationToken)
                    {
                        _login = _loginService.LoadLogin(archivoYReceptor.TokenEnterprise!, archivoYReceptor.TokenPassword!);
                        expirationToken = true;
                    }

                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "LoadLogin", LevelMsn.Info, 0);

                    if (expirationToken)
                    {
                        logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"loadLoginVar.Result.passwordExpiration = {_login.passwordExpiration}", LevelMsn.Info, 0);

                        token = _login.token!;
                        if (!string.IsNullOrEmpty(token))
                        {
                            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Se toma el token de login", LevelMsn.Info, 0);
                        }

                        //*******Radicar archivo XML*******//
                        IEnviarXMLResponse responseEnviarXML = await _enviarXml.LoadEnviarXML(token, archivoYReceptor);



                        logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name,
                            $"LoadEnviarXML {archivoYReceptor.NombreXml} Resultado= {responseEnviarXML.resultado} Codigo= {responseEnviarXML.codigo}" +
                            $" Mensaje= {responseEnviarXML.documentoId} {responseEnviarXML.mensaje}", LevelMsn.Error, 0);

                        archivoYReceptor.Codigo = responseEnviarXML.codigo.ToString();
                        archivoYReceptor.TipoDocumento = responseEnviarXML.TipoDocumento;

                        if (responseEnviarXML.codigo == 200 || responseEnviarXML.codigo == 201)
                        {
                            respuesta.Resultado = true;
                            respuesta.Codigo = responseEnviarXML.codigo;
                            respuesta.Detalles = responseEnviarXML.mensaje;
                            respuesta.Descripcion = responseEnviarXML.documentoId;

                            if (string.IsNullOrEmpty(archivoYReceptor.nitEmisor) || string.IsNullOrEmpty(archivoYReceptor.NumDocumento))
                            {
                                archivoYReceptor.nitEmisor = responseEnviarXML.numeroIdentificacion;
                                archivoYReceptor.NumDocumento = responseEnviarXML.documentoId;
                                archivoYReceptor.TipoIdentificacionEmisor = responseEnviarXML.tipoIdentificacion;
                            }

                            if (IUtils.TipoIdentificacion(archivoYReceptor.TipoIdentificacionEmisor!))
                            {
                                //*******Radicar representación grafica*******//
                                IRespuestaRadicacion enviadoRepGrafica = await _enviarRepresentacion.LoadEnviarRepGrafica(token, archivoYReceptor);


                                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "LoadEnviarRepGrafica Resultado="
                                                                                                 + enviadoRepGrafica.resultado + " Codigo=" + enviadoRepGrafica.codigo.ToString()
                                                                                                 + " Mensaje=" + enviadoRepGrafica.mensaje, LevelMsn.Info, 0);

                                if (!string.IsNullOrEmpty(archivoYReceptor.Adjunto))
                                {
                                    //*******Radicar archivos adjuntos*******//
                                    IRespuestaRadicacion enviadoAnexo = await _enviarAnexo.LoadEnviarAnexo(token, archivoYReceptor);


                                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "LoadEnviarAnexo Resultado="
                                                                                                + enviadoAnexo.resultado + " Codigo=" + enviadoAnexo.codigo.ToString()
                                                                                                + " Mensaje=" + enviadoAnexo.mensaje, LevelMsn.Info, 0);
                                }
                            }
                            else
                            {
                                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"El tipo de documento {archivoYReceptor.TipoIdentificacionEmisor} del emisor {archivoYReceptor.nitEmisor} no es correcto, se envia XML sin representación grafica", LevelMsn.Info, 0);
                            }
                        }
                        else
                        {
                            respuesta.Resultado = true;
                            respuesta.Codigo = responseEnviarXML.codigo;
                            respuesta.Detalles = responseEnviarXML.mensaje;
                        }
                    }

                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Proceso de radicación culminado " + archivoYReceptor.NombreXml + " Codigo=" + archivoYReceptor.Codigo, LevelMsn.Info, 0);
                    try
                    {
                        if (lRecErrores.Contains(Convert.ToInt32(archivoYReceptor.Codigo)))
                        {
                            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Proceso de radicacion completado  " + archivoYReceptor.NombreXml, LevelMsn.Info, 0);
                        }
                        else
                        {
                            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "El codigo " + archivoYReceptor.Codigo + " Requiere que vuelva a radicarse el documento " + archivoYReceptor.NombreXml, LevelMsn.Info, 0);
                        }
                    }
                    catch (Exception ex)
                    {
                        msg = "Archivo " + archivoYReceptor.Ruta + " Excepción al procesar " + ex.Message;
                        logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, msg, LevelMsn.Error, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                msg = "Excepción en Metodo RUN " + ex.Message;
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, msg, LevelMsn.Error, 0);
                archivoYReceptor.Codigo = "500";
                respuesta.Resultado = false;
                respuesta.Codigo = 500;
                respuesta.Detalles = "Error";
            }
            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name,
                                        string.Format("Resultado:\n  Codigo: \"{0}\", Descripcion: \"{1}\", Detalles: \"{2}\"", respuesta.Codigo, respuesta.Descripcion, respuesta.Detalles),
                                        LevelMsn.Info, 0);
            if (archivoYReceptor.Codigo!.Equals("115"))
            {
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Consulta saldo transacciones  Codigo= {respuesta.Codigo} Mensaje= {respuesta.Detalles}", LevelMsn.Info, 0);
            }

            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Proceso de radicacion completado  {archivoYReceptor.NombreXml}", LevelMsn.Info, 0);
            return respuesta;
        }
    }
}
