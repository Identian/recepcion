using CapaDominio.Enums.Logs;
using CapaDominio.Enums.TipoAutenticacion;
using CapaDominio.Interfaces.IHelpers;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MoreLinq;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Infraestructura.Services.GestionEmail
{
    public class EmailManagement : IEmailManagement
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailConectar _conectar;
        private readonly IProcesoRecepcionMensajes _procesoRecepcionMensajes;
        

        public EmailManagement(IConfiguration configuration, IEmailConectar conectar, IProcesoRecepcionMensajes procesoRecepcionMensajes)
        {
            _configuration = configuration;
            _conectar = conectar;
            _procesoRecepcionMensajes = procesoRecepcionMensajes;
        }

        public async Task<IRespuesta> ProcesarMensajesPendientes(IReceptorBase infoReceptorBase, ICuentaCorreo infoCorreoReceptor, ILogAzure logAzure)
        {
            IRespuesta _respuesta = new Respuesta();

            _respuesta.SetMetodo($"{nameof(EmailManagement)} . {nameof(ProcesarMensajesPendientes)}");

            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "begin " + infoCorreoReceptor.Usuario, LevelMsn.Info, 0);

            infoCorreoReceptor.Clave = IUtils.DesEncriptar(infoCorreoReceptor.Clave!);

            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de conectar al buzon " + infoCorreoReceptor.Usuario, LevelMsn.Info, 0);

            _respuesta = await _conectar.Conectar(infoCorreoReceptor, logAzure);

            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Resultado de conectar al buzon Codigo= " + _respuesta.Codigo.ToString() +
                " Descripcion = " + _respuesta.Descripcion + " - " + _respuesta.Detalles, LevelMsn.Info, 0);
            Stopwatch timeT = new();
            if (_respuesta.Resultado)
            {
                //Despues de conectarme retorno el objeto de conexión al correo
                using ImapClient client = (ImapClient)_respuesta.ValorObject[0];
                //Remuevo el cliente
                _respuesta.ValorObject.RemoveAt(0);

                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Buscando las carpetas dentro del buzon de correo para " + infoCorreoReceptor.Usuario, LevelMsn.Info, 0);
                _respuesta = await _procesoRecepcionMensajes.OpenFoldersTFHKA(infoCorreoReceptor.Usuario!, client, logAzure);

                //Validación de resultado de apertura de carpetas
                if (_respuesta.Resultado)
                {    
                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    if (infoCorreoReceptor.TipoAutenticacion == TipoAutenticacion.AUTENTICACION_BASICA)
                    {
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                    }
                    bool hayMensajes = false;

                    //Tiempo maximo que permite el procesamiento
                    //Evitamos que las cuentas se queden pegados.
                    //2 minutos
                    int tiempoLimite = _configuration.GetValue<int>("GestionEmail:MaxTiempoPorCuenta", 180000);

                    System.Timers.Timer tiempoMaximoDeLectura = new(tiempoLimite)
                    {
                        AutoReset = false
                    };
                    tiempoMaximoDeLectura.Start();

                    try
                    {
                        //Vamos a construir una estructura para almacenar las carpetas del correo a leer
                        List<IMailFolder> listaDeBuzonesAProcesar = new();
                        bool esBandejaEntrada = true; //declaración para saber si la carpeta es Bandeja de Entrada o no
                        listaDeBuzonesAProcesar.Add((IMailFolder)_respuesta.ValorObject[0]);  //Bandeja de entrada es el 1er elemento en añadir

                        try
                        {
                            if (_respuesta.ValorObject.Count >= 4)
                            {
                                listaDeBuzonesAProcesar.Add((IMailFolder)_respuesta.ValorObject[3]);
                            }
                            else
                            {
                                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name,
                                "No se pudo encontrar la carpeta SPAM para e4" +
                                infoCorreoReceptor.Usuario + LevelMsn.Info, 0);
                            }

                        }
                        catch (Exception e4)
                        {
                            Debug.WriteLine(e4.StackTrace);
                            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name,
                                "No se pudo encontrar la carpeta SPAM para e4" +
                                infoCorreoReceptor.Usuario + " Excepcion: " +
                                e4.Message, LevelMsn.Info, 0);
                        }
                        StringBuilder sbDetallesAdicionales = new();
                        logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de recorrer los buzones .", LevelMsn.Info, 0);
                        IRespuesta respuesta = new Respuesta();
                        foreach (IMailFolder buzonActual in listaDeBuzonesAProcesar)
                        {
                            try
                            {
                                await buzonActual.OpenAsync(FolderAccess.ReadWrite);
                                IList<UniqueId> listaDeIDsDeMensajesDeLaBandejaActual = new List<UniqueId>();
                                if (esBandejaEntrada)
                                {
                                    listaDeIDsDeMensajesDeLaBandejaActual = await buzonActual.SearchAsync(SearchQuery.All); //Todos -- Bandeja de Entrada
                                }
                                else
                                {
                                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name,
                                                "Buscando la carpeta SPAM para " + infoCorreoReceptor.Usuario,
                                                LevelMsn.Info, 0);
                                    listaDeIDsDeMensajesDeLaBandejaActual = listaDeIDsDeMensajesDeLaBandejaActual.Count == 0 ? await buzonActual.SearchAsync(SearchQuery.NotSeen) : listaDeIDsDeMensajesDeLaBandejaActual;
                                    //Solo los no leidos -- SPAM
                                }

                                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de recorrer la lista de correos para la bandeja .", LevelMsn.Info, 0);


                                int batchSize = _configuration.GetValue<int>("GestionEmail:MaxCorreoAprocesar", 50);
                                IEnumerable<IEnumerable<UniqueId>> loteCorreos = listaDeIDsDeMensajesDeLaBandejaActual.Batch(batchSize);

                                if (loteCorreos.Any())
                                {
                                    int count = 0;
                                    foreach (UniqueId idMensajeActual in loteCorreos.First())
                                    {
                                        timeT.Start();
                                        count++;
                                        try
                                        {
                                            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name,
                                                string.Format("Carpeta: \"{0}\" -  - Antes de leer email ", buzonActual.FullName), LevelMsn.Info, 0);
                                            MimeMessage mensajeAProcesar = await buzonActual.GetMessageAsync(idMensajeActual);
                                            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name,
                                               string.Format("Carpeta: \"{0}\" - Asunto Email: \"{1}\"", buzonActual.FullName, mensajeAProcesar.Subject), LevelMsn.Info, 0);


                                            // ProcesarMensaje
                                            respuesta = await _procesoRecepcionMensajes.RadicarMensaje(
                                                  infoCorreoReceptor.Usuario!,
                                                  idMensajeActual,
                                                  mensajeAProcesar,
                                                  _respuesta.ValorObject[1],
                                                  _respuesta.ValorObject[2],
                                                  buzonActual,
                                                  infoReceptorBase,
                                                  logAzure,
                                                  esBandejaEntrada);

                                            if (!tiempoMaximoDeLectura.Enabled)
                                            {
                                                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Proceso terminado por timeOut", LevelMsn.Info, 0);
                                                logAzure.SaveLog(_respuesta.Codigo.ToString(), "Carga de documentos", infoReceptorBase.NumeroIdentificacionReceptor!, "AgeRadicacion", "Proceso terminado por timeOut", infoCorreoReceptor.Usuario!, ref timeT);
                                                break;
                                            }

                                            //En el caso que el cliente no tenga transacciones disponibles se termina el proceso
                                            if (respuesta.TerminarProceso)
                                            {
                                                break;
                                            }

                                            if (!string.IsNullOrEmpty(_respuesta.DetallesAdicionales)) //Si hay algo que añadir
                                            {
                                                if (sbDetallesAdicionales.Length > 0) //Comprobamos si el string Builder ya tiene elementos
                                                {
                                                    sbDetallesAdicionales.Append(" -- ");
                                                }
                                                sbDetallesAdicionales.Append(_respuesta.DetallesAdicionales);
                                            }
                                        }
                                        catch (Exception e3)
                                        {
                                            _respuesta.Codigo = 500;
                                            _respuesta.Descripcion = string.Format("Problemas al leer el mensaje con ID: \"{0}\" en la carpeta: \"{1}\" e3", idMensajeActual.Id, buzonActual.FullName);
                                            _respuesta.Detalles = e3.Message;
                                        }
                                        logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name,
                                        string.Format("Resultado:\n  Codigo: \"{0}\", Descripcion: \"{1}\", Detalles: \"{2}\" e3", _respuesta.Codigo, _respuesta.Descripcion, _respuesta.Detalles),
                                        LevelMsn.Info, 0);
                                    }

                                    if (respuesta.TerminarProceso)
                                    {
                                        _respuesta.MensajesProcesados = -1;
                                        break;
                                    }


                                    if (count >= 1)
                                    {
                                        _respuesta.MensajesProcesados = count;
                                        break;
                                    }
                                    else
                                    {
                                        _respuesta.MensajesProcesados = count;
                                    }
                                }
                            }
                            catch (Exception e2)
                            {
                                _respuesta.Codigo = 500;
                                _respuesta.Descripcion = string.Format("Problemas accesando carpeta: \"{0}\" e2", buzonActual.FullName);
                                _respuesta.Detalles = e2.Message;
                            }

                            esBandejaEntrada = false;
                        }

                        _respuesta.DetallesAdicionales = sbDetallesAdicionales.ToString();

                        if (sbDetallesAdicionales.Length > 0 && !hayMensajes)
                        {
                            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name,
                                            "Procesadas la bandeja de entrada y SPAM.", LevelMsn.Info, 0);
                        }
                        else
                        {
                            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name,
                                            "No se encontraron mensajes en la bandeja de entrada y SPAM.", LevelMsn.Info, 0);
                            _respuesta.DetallesAdicionales = "No se encontraron mensajes en la bandeja de entrada y SPAM.";
                        }
                    }
                    catch (Exception e1)
                    {
                        _respuesta.Codigo = 500;
                        _respuesta.Descripcion = "Problemas al leer las carpetas del correo e1";
                        _respuesta.Detalles = e1.Message;
                    }
                    finally
                    {
                        tiempoMaximoDeLectura.Dispose();
                    }

                    client.Disconnect(true);
                }
            }
            return _respuesta;
        }
    }
}
