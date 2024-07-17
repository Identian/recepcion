using CapaDominio.Enums.Logs;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using Infraestructura.Helpers.Config;
using Infraestructura.Logs;
using Infraestructura.Services.DocumentosElectronico;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MoreLinq;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Infraestructura.Services.RecepcionDeMensajes
{
    public class ProcesoRecepcionMensajes : IProcesoRecepcionMensajes
    {
        private readonly IConfiguration _configuration;
        private readonly IProcesoRecepcionDocumentoZip _procesoRecepcionDocumentoZip;
        protected int idEjecucion;
        public ProcesoRecepcionMensajes(IConfiguration configuration, IProcesoRecepcionDocumentoZip procesoRecepcionDocumentoZip)
        {
            _configuration = configuration;
            _procesoRecepcionDocumentoZip = procesoRecepcionDocumentoZip;
        }

        /// <summary>
        /// Metodo que permite explorar las carpetas del correo que se este analizando buscando las 
        /// Que hacen match con TFHKA esto evitando abrir una a una y sin depeneder dela estructura 
        /// de cada correo, algunas los separadores son . o / o las carpetas empiezan con INBOX
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="clienteImap"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<IRespuesta> OpenFoldersTFHKA(string usuario, ImapClient clienteImap, ILogAzure log)
        {
            IRespuesta result = new Respuesta();
            result.SetMetodo($"Proceso apertura de carpetas . {nameof(AbrirBandeja)}");
            string carpeta = "";

            List<IMailFolder?> rutasFolder = new();
            IList<IMailFolder> folders = new List<IMailFolder>();
            List<string> spamFolders = new() { "Bulk Mail", "[Gmail]/Spam", "Junk", "Bulk", "Spam", "Correo no deseado" };
            try
            {
                try
                {
                    folders = await clienteImap.GetFoldersAsync(clienteImap.PersonalNamespaces[0]);

                    //Inicalmente la carpeta Inbox es la primera en agregarse
                    rutasFolder.Add(clienteImap.Inbox);

                    result.Detalles = $"Abrir Carpeta {clienteImap.Inbox} Acceso OK";
                    result.Codigo = 200;
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name, result.Detalles, LevelMsn.Info, 0);
                }
                catch (Exception of2)
                {
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"No se obtuvo acceso a las carpetas del correo: {usuario}, Exception: { JsonConvert.SerializeObject(of2)}", LevelMsn.Info, 0);
                    result.Codigo = 500;
                    result.Descripcion = $"Error en la lectura de carpetas del correo: {usuario}, no se pueden obtener.";
                    result.Resultado = false;
                    return result;
                }

                if (folders.Count > 0)
                {
                    IMailFolder? inboxFolder = folders.FirstOrDefault();
                       
                    //Comprobar si existen prefijos en los nombre de las carpetas ejemplo: INBOX.Inbox
                    char separador = clienteImap.PersonalNamespaces[0].DirectorySeparator;
                    string? prefijo = GetPrefixFolder(separador, inboxFolder?.FullName);

                    //Obtener carpetas de TFHKA
                    foreach (IMailFolder folder in folders)
                    {
                        carpeta = folder.FullName;
                        if (carpeta.Contains($"TFHKA{separador}Recepcion{separador}"))//Se agregan las carpetas de TFHKA
                        {
                            IMailFolder? route = Rutas(folder, prefijo);
                            if (route != null)
                            {
                                rutasFolder.Add(route);
                            }
                        }
                    }

                    //Aqui se debe evaluar si no se encontraron las carpetas de TFHKA se deben crear

                    if (rutasFolder.Count == 1)
                    {
                        string? carpetaDescargados = _configuration.GetValue<string>("Bandejas:BandejaDescargados", "TFHKA/Recepcion/Descargados");
                        string? carpetaErroneos = _configuration.GetValue<string>("Bandejas:BandejaErroneos", "TFHKA/Recepcion/Erroneos");
                        string?[] carpetas = new[] { carpetaDescargados, carpetaErroneos };

                        foreach (string? createFolder in carpetas)
                        {
                            this.CrearCarpeta(clienteImap, createFolder!);
                        }
                        //Se usa recursividad para ejecutarse de nuevo el metodo y obtener las carpetas correctamente.
                        result = await OpenFoldersTFHKA(usuario, clienteImap, log);
                        return result;
                    }

                    //Obtener carpeta de spam
                    IMailFolder? spamFolder = folders.FirstOrDefault(folder =>
                    spamFolders.Any(spamName =>
                        folder.FullName.EndsWith($"{prefijo ?? ""}{spamName}", StringComparison.OrdinalIgnoreCase)
                    ));

                    if (spamFolder != null)
                    {
                        rutasFolder.Add(spamFolder);
                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Carpeta spam encontrada: {spamFolder!.FullName}.", LevelMsn.Info, 0);
                    }
                    else
                    {
                        try
                        {
                            IMailFolder spam = await clienteImap.GetFolderAsync(SpecialFolder.Junk.ToString());
                            rutasFolder.Add(spam);
                            log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Carpeta: {spam!.FullName} abierta correctamente.", LevelMsn.Info, 0);
                        }
                        catch (FolderNotFoundException of1)
                        {
                            log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Carpeta: Spam no encontrada: {of1.Message}. of1", LevelMsn.Info, 0);
                        }
                    }

                    //Verificar que el orden de las carpetas sea correcto antes de retornar la lista.
                    if (rutasFolder.Count > 2 && rutasFolder[1] != null && rutasFolder[2] != null)
                    {
                        if (rutasFolder[1]!.FullName != null && rutasFolder[1]!.FullName.Contains("Erroneos"))
                        {
                            IMailFolder cambio = rutasFolder[1]!;
                            rutasFolder[1] = rutasFolder[2];
                            rutasFolder[2] = cambio;
                        }
                    }



                    //Abrir las carpetas encontradas y anexadas a lista
                    rutasFolder.ForEach(r =>
                    {
                        IMailFolder folder = clienteImap.GetFolder(r!.FullName);
                        r.Open(FolderAccess.ReadWrite);
                        result.Resultado = true;
                        result.ValorObject.Add(folder);
                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Carpeta: {r!.FullName} abierta correctamente.", LevelMsn.Info, 0);
                    });
                }
                else
                {
                    result.Codigo = 500;
                    result.Descripcion = $"No se pudo obtener las carpetas del correo electronico: {usuario}";
                    result.Resultado = false;
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name, result.Descripcion, LevelMsn.Error, 0);
                }
            }
            catch (Exception of2)
            {
                result.Codigo = 500;
                result.Descripcion = "No se pudo obtener acceso a la carpeta '" + carpeta + "' en la cuenta de correo '" + usuario + "' of2";
                result.Resultado = false;
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(of2), LevelMsn.Error, 0);
                return result;
            } 
            return result;
        }

        /// <summary>
        /// Metodo que se encarga de extraer las carpetas que contengan el nombre de TFHKA/Recepcion/Descargados
        /// Funciona solo con otra subcarpeta luego de Recepción
        /// </summary>
        /// <param name="carpeta"></param>
        /// <returns></returns>
        private static IMailFolder? Rutas(IMailFolder carpeta, string? prefijo)
        {
            string patron = @"^TFHKA[./]Recepcion(?:[./][^./]+)?$";
            if (!string.IsNullOrEmpty(prefijo))
            {
                patron = $@"^{Regex.Escape(prefijo)}TFHKA[./]Recepcion(?:[./][^./]+)?$";
            }
            Regex regex = new(patron);
            bool match = regex.IsMatch(carpeta.FullName!);
            IMailFolder? successFolder = null;
            if (match)
            {
                successFolder = carpeta;
                return successFolder;
            }
            return successFolder;
        }

        /// <summary>
        /// Metodo que valida si existen nombres compuestos en las carpetas del servidor
        /// </summary>
        /// <param name="separador"></param>
        /// <param name="cadena"></param>
        /// <returns></returns>
        private static string? GetPrefixFolder(char separador, string? cadena)
        {
            if (cadena != null)
            {
                string[] resultado = cadena.Split(separador);
                return resultado.Length > 1 ? $"{resultado[0]}{separador}" : string.Empty;
            }
            return string.Empty;
        }

        public IRespuesta AbrirBandeja(string carpeta, string usuario, ImapClient clienteImap, ILogAzure log)
        {
            IRespuesta result = new Respuesta();
            result.SetMetodo($"ProcesoRecepcionAdjuntos . {nameof(AbrirBandeja)}");
            try
            {
                string resultDir = "";
                clienteImap.Timeout = int.Parse(_configuration.GetSection("Imap:TimeOut").Value ?? "30000");

                if (!(carpeta.ToLower().Contains("spam") || carpeta.ToLower().Contains("Bulk") || carpeta.ToLower().Contains("Junk")))
                {
                    //Llamado a la funcion que permite validar si la ruta existe
                    bool existe = FolderExistOrCret(clienteImap, carpeta);
                    if (!existe)
                    {
                        //Llamado a la funcion que crea los directorios
                        resultDir = CrearCarpeta(clienteImap, carpeta);
                    }
                }
                try
                {
                    IMailFolder bandeja = clienteImap.GetFolder(carpeta);
                    bandeja.Open(FolderAccess.ReadWrite);
                    result.Resultado = true;
                    result.Codigo = 200;
                    result.ValorObject.Add(bandeja);
                    if (resultDir != "")
                    {
                        result.Descripcion = resultDir;
                    }
                    else
                    {
                        result.Descripcion = "Abrir Carpeta " + carpeta + " Acceso ok";
                    }
                }
                catch (Exception e)
                {
                    result.Codigo = 500;
                    result.Descripcion = "No se pudo abrir la carpeta '" + carpeta + "' en la cuenta de correo '" + usuario + "'";
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Codigo = 500;
                result.Descripcion = "No se pudo obtener acceso a la carpeta '" + carpeta + "' en la cuenta de correo '" + usuario + "'";
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
                return result;
            }
            if (!result.Resultado)
            {
                result.Detalles = "Fallo al abrir Carpeta=" + carpeta + ";Usuario=" + usuario;
                result.Codigo = 500;
            }
            return result;
        }

        public string CrearCarpeta(ImapClient imapC, string carpeta)
        {
            string[] carpetas = carpeta.ToString().Split("/");          /*Se realiza split de la ruta en caso de que contenga varias subcarpetas*/
            IMailFolder foldersOnEmail = imapC.GetFolder(imapC.PersonalNamespaces[0]);
            try
            {

                if (carpetas.Length == 1)
                {
                    foldersOnEmail.Create(carpeta.ToString(), true); //De esta forma se crea una carpeta principal
                    return "Directorio " + carpeta.ToString() + " creado satisfactoriamente";
                }
                else
                {
                    string rutaFolders = "";
                    for (int i = 0; i < carpetas.Length - 1; i++)
                    {
                        //condicion luego de incrementar
                        rutaFolders = $"{rutaFolders} {carpetas[i]}";

                        if (!FolderExistOrCret(imapC, rutaFolders)) //valido se la ruta que se va modificando existe
                        {
                            foldersOnEmail = foldersOnEmail.Create(carpetas[i].ToString(), false); /*se crean las diferentes subcarpetas de acuerdo a la ruta establecida previamente*/
                            rutaFolders = $"{rutaFolders}/";
                        }
                        else
                        {
                            foldersOnEmail = imapC.GetFolder(new FolderNamespace('/', rutaFolders.Replace(" ", "")));
                            rutaFolders = $"{rutaFolders} /";
                        }
                    }
                    foldersOnEmail = foldersOnEmail.Create(carpetas[carpetas.Length - 1], true);
                    return "Directorio " + carpeta.ToString() + " creado satisfactoriamente";
                }

            }
            catch (Exception)
            {
                return "Error al crear el directorio " + carpeta.ToString();
            }
        }

        public bool FolderExistOrCret(ImapClient imapC, string path)
        {
            path = path.Replace(" ", "").Replace("\t", "").Replace("\n", "");
            bool exist = false;
            IList<IMailFolder> folderOnEmail = imapC.GetFolders(imapC.PersonalNamespaces[0]); //Carpetas del cliente de correo
            foreach (string folder in folderOnEmail.Select(folder => folder.FullName))
            {
                try
                {
                    int res = String.Compare(folder, path.Trim(), StringComparison.OrdinalIgnoreCase);
                    if (res == 0)
                    {
                        imapC.GetFolder(folder);
                        exist = true;
                        break;
                    }
                }
                catch (FolderNotFoundException)
                {
                    return false;
                }
            }
            return exist;
        }

        public IRespuesta LeerUltimoMensaje(object bandejaEntrada, ILogAzure log, int elem = -1)
        {
            IRespuesta result = new Respuesta();
            result.SetMetodo($"ProcesoRecepcionAdjuntos . {nameof(LeerUltimoMensaje)}");
            try
            {
                IMailFolder bandeja = (IMailFolder)bandejaEntrada;
                IMessageSummary summary;
                if (elem == -1)
                {
                    summary = bandeja.Fetch(bandeja.Count - 1, bandeja.Count - 1, MessageSummaryItems.UniqueId).FirstOrDefault();
                }
                else
                {
                    summary = bandeja.Fetch(elem, elem, MessageSummaryItems.UniqueId).FirstOrDefault();
                }

                UniqueId ultimoUid = summary != null ? summary.UniqueId : UniqueId.Invalid;
                if (ultimoUid != UniqueId.Invalid)
                {
                    try
                    {
                        MimeMessage ultimoMensaje = bandeja.GetMessage(ultimoUid);
                        result.Resultado = true;
                        result.ValorObject.Add(ultimoUid);
                        result.ValorObject.Add(ultimoMensaje);

                    }
                    catch (Exception e)
                    {
                        result.Codigo = 999;
                        result.Descripcion = "No se pudo obtener el contenido del mensaje de correo";
                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
                        return result;
                    }
                }
                else
                {
                    result.Codigo = 99;
                    result.Descripcion = "El código único IMAP del mensaje de correo no es válido";
                }
            }
            catch (Exception e)
            {
                result.Codigo = 999;
                result.Descripcion = "No se pudo consultar la bandeja del buzón de correo";
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
                return result;
            }
            return result;
        }

        public IRespuesta MarcarMensajeLeido(object ultimoUid, object bandejaMensaje, ILogAzure log)
        {
            IRespuesta result = new Respuesta();
            result.SetMetodo($"ProcesoRecepcionAdjuntos . {nameof(MarcarMensajeLeido)}");
            try
            {
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Marcando email como leido ", LevelMsn.Info, 0);
                ((IMailFolder)bandejaMensaje).AddFlags((UniqueId)ultimoUid, MessageFlags.Seen, true);
                result.Resultado = true;
                result.Codigo = 200;
                result.Descripcion = " Marcado como leído el correo ";
                return result;
            }
            catch (Exception e)
            {
                result.Codigo = 500;
                result.Descripcion = "No se pudo colocar la marca de leído al mensaje de correo";
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
                return result;
            }
        }

        public IRespuesta MoverMensaje(object ultimoUid, object bandejaOrigen, object bandejaDestino, ILogAzure log)
        {
            IRespuesta result = new Respuesta();
            result.SetMetodo($"ProcesoRecepcionAdjuntos . {nameof(MoverMensaje)}");
            UniqueId? uniqueId = ((IMailFolder)bandejaOrigen).MoveTo((UniqueId)ultimoUid, (IMailFolder)bandejaDestino);

            if (uniqueId == null)
            {
                result.Codigo = 500;
                result.Descripcion = "No se pudo mover el mensaje";
                result.Detalles = "Origen " + bandejaOrigen.ToString() + " Destino " + bandejaDestino.ToString();
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "No se pudo mover el mensaje ", LevelMsn.Info, 0);
            }
            else
            {
                result.Codigo = 200;
                result.Descripcion = "Mensaje movido";
                result.Detalles = "Origen " + bandejaOrigen.ToString() + " Destino " + bandejaDestino.ToString();
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "El mensaje se movio - " + result.Detalles, LevelMsn.Info, 0);
            }
            return result;
        }

        /// <summary>
        /// Metodo encargado de iniciar proceso de radicación de los documentos
        /// Metodo llamado en la clase EmailManagement es el que se encarga de extaer y enviar los documentos al servicio
        /// de recepción
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="remitentes"></param>
        /// <param name="ultimoUid"></param>
        /// <param name="ultimoMensaje"></param>
        /// <param name="bandejaDescargados"></param>
        /// <param name="bandejaErroneos"></param>
        /// <param name="pCarpetaDeEmailActual"></param>
        /// <param name="directorio"></param>
        /// <param name="infoContribuyente"></param>
        /// <param name="log"></param>
        /// <param name="esBandejaEntrada"></param>
        /// <returns></returns>
        public async Task<IRespuesta> RadicarMensaje(string usuario, object ultimoUid, object ultimoMensaje, object bandejaDescargados, object bandejaErroneos, object pCarpetaDeEmailActual, IReceptorBase infoContribuyente, ILogAzure log, bool esBandejaEntrada)
        {
            Stopwatch timeT = new();
            timeT.Start();

            ILogAzure logAzure = new LogAzure(_configuration, infoContribuyente.IdReceptor.ToString());

            UniqueId uid = (UniqueId)ultimoUid;
            using MimeMessage mensaje = (MimeMessage)ultimoMensaje;

            IMailFolder descargadosTFHKA = (IMailFolder)bandejaDescargados;
            IMailFolder erroneosTFHKA = (IMailFolder)bandejaErroneos;
            IMailFolder carpetaDeEmailActual = (IMailFolder)pCarpetaDeEmailActual;


            MensajeBase mensajeCorreo = new(idEjecucion)
            {
                IdEjecucion = idEjecucion,
                IdUnicoImap = uid.Id,
                Usuario = Config.GetUsuarioCorreo(usuario),
                Asunto = mensaje.Subject,
                FechaHora = mensaje.Date,
                Remitente = Config.GetUsuarioCorreo(mensaje.From.ToString()),
                CarpetaOrigen = carpetaDeEmailActual.FullName,
                Destinatarios = Config.GetListaUsuariosCorreo(mensaje.To.ToString())
            };
            IRespuesta result = await _procesoRecepcionDocumentoZip.RadicarZip(mensaje, infoContribuyente, logAzure, usuario);

            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Resultado proceso de radicación documentos " + result.Detalles + " -- " + result.Descripcion, LevelMsn.Info, 0);

            switch (result.Codigo)
            {
                case 200:
                case 201:
                case 76:
                case 87:
                case 83:
                case 82:
                case 84:
                case 85:
                case 89:
                case 106:
                case 103:
                case 104:
                case 105:
                    if (esBandejaEntrada)
                    {
                        MoverPorCodigo(result, mensajeCorreo, uid, carpetaDeEmailActual, descargadosTFHKA, logAzure);
                        logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Se mueve correo a bandeja Descargados", LevelMsn.Info, 0);
                    }
                    else
                    {
                        MoverPorCodigoError(uid, mensajeCorreo, carpetaDeEmailActual, erroneosTFHKA, logAzure, esBandejaEntrada);
                        logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Se mueve correo a bandeja erroneos", LevelMsn.Info, 0);
                    }
                    break;
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 500:
                    MoverPorCodigoError(uid, mensajeCorreo, carpetaDeEmailActual, erroneosTFHKA, logAzure, esBandejaEntrada);
                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Se mueve correo a bandeja Erroneos", LevelMsn.Info, 0);
                    break;
                case 115:
                    result.TerminarProceso = true;
                    break;
                default:
                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"El mensaje no se mueve de carpeta Codigo={result.Codigo} Mensaje={result.Detalles}", LevelMsn.Info, 0);
                    break;
            }

            logAzure.SaveLog(result.Codigo.ToString()!, "Carga de documentos", infoContribuyente.NumeroIdentificacionReceptor!, "AgeRadicacion", $"{result.Descripcion ?? result.Detalles}", usuario!, ref timeT);
            return result;
        }

        /// <summary>
        /// Mover los mensajes de descargados
        /// </summary>
        /// <param name="result"></param>
        /// <param name="mensajeCorreo"></param>
        /// <param name="uid"></param>
        /// <param name="carpetaDeEmailActual"></param>
        /// <param name="carpetaFinal"></param>
        /// <param name="logAzure"></param>
        public void MoverPorCodigo(IRespuesta result, MensajeBase mensajeCorreo, UniqueId uid, IMailFolder carpetaDeEmailActual, IMailFolder carpetaFinal, ILogAzure logAzure)
        {
            result.DetallesAdicionales = mensajeCorreo.DocumentoElectronicoXML.Recibido; //En Detalles adicionales vamos colocando el Nombre del XML que se bajó 
            mensajeCorreo.IdTipoMensaje = 1;
            logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Se mueve correo a bandeja Descargados", LevelMsn.Info, 0);
            MoverMensaje(uid, carpetaDeEmailActual, carpetaFinal, logAzure);  //Incluso si es SPAM, debe moverse a descargados como No leído ya que es valido
            mensajeCorreo.CarpetaDestino = carpetaFinal.FullName;
        }

        /// <summary>
        /// Mover los correos a erroneos
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="mensajeCorreo"></param>
        /// <param name="carpetaDeEmailActual"></param>
        /// <param name="carpetaFinal"></param>
        /// <param name="logAzure"></param>
        /// <param name="esBandejaDeEntrada"></param>
        public void MoverPorCodigoError(UniqueId uid, MensajeBase mensajeCorreo, IMailFolder carpetaDeEmailActual, IMailFolder carpetaFinal, ILogAzure logAzure, bool esBandejaDeEntrada)
        {
            mensajeCorreo.IdTipoMensaje = 2;
            mensajeCorreo.CarpetaDestino = carpetaFinal.FullName;
            if (esBandejaDeEntrada) //Mover a erroneos si el correo está en Bandeja de Entrada
            {
                MoverMensaje(uid, carpetaDeEmailActual, carpetaFinal, logAzure);
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Se mueve correo a bandeja Erroneos", LevelMsn.Info, 0);
            }
            else //Si es una carpeta distinta de Bandeja de Entrada, no mover, pero marcar como leído
            {
                MarcarMensajeLeido(uid, carpetaDeEmailActual, logAzure);
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, string.Format("Correo Erroneo. Se deja en \"{0}\" como leído", carpetaDeEmailActual.FullName), LevelMsn.Info, 0);
            }
        }

    }
}