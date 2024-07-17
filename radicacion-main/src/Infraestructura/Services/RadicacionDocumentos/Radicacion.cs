using CapaDominio.Enums.Logs;
using CapaDominio.Interfaces.IHelpers;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Radicacion;
using CapaDominio.Response;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

namespace Infraestructura.Services.RadicacionDocumentos
{
    /// <summary>
    /// Clase encargad de radicar los archivos contenidos en el archivo .zip justo despues de la lectura del correo
    /// </summary>
    public class Radicacion : IRadicacion
    {
        private readonly IConfiguration _configuration;
        private readonly IMultiservices _multiservices;
        private readonly IDataBaseHelper _dataBaseHelper;

        public Radicacion(IConfiguration configuration, IMultiservices multiservices, IDataBaseHelper dataBaseHelper)
        {
            _configuration = configuration;
            _multiservices = multiservices;
            _dataBaseHelper = dataBaseHelper;
        }

        /// <summary>
        /// Metodo encargado de leer y validar que en el archivo .zip exista un archivo XML valido y si es asi
        /// Procede con procesamiento y carga del documento
        /// </summary>
        /// <param name="mensaje"></param>
        /// <param name="infoContribuyente"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<IRespuesta> RadicarArchivoAsync(MimeMessage mensaje, IReceptorBase infoContribuyente, ILogAzure log, string usuario)
        {
            Stopwatch timeT = new();
            timeT.Start();
            IRespuesta respuesta = new Respuesta();
            respuesta.SetMetodo($"{nameof(Radicacion)} . {nameof(RadicarArchivoAsync)}");
            string namefile = "";
            try
            {
                IEnumerable<MimePart> attachments = mensaje.BodyParts.OfType<MimePart>().Where(part => !string.IsNullOrEmpty(part.FileName));
                foreach (MimeEntity attachment in attachments)
                {
                    using MimePart part = (MimePart)attachment;

                    if (Path.GetExtension(part.FileName).ToLower().Equals(".zip"))
                    {
                        using MemoryStream memoryStream = new();
                        await part.Content.DecodeToAsync(memoryStream);
                        memoryStream.Position = 0;
                        using ZipArchive archive = new(memoryStream, ZipArchiveMode.Read);
                        namefile = part.FileName;
                        if (archive.Entries.Count > 0)
                        {
                            //Se valida que dentro del archivo .zip exista un archivo XML 
                            bool contieneArchivoXml = archive.Entries.Any(entry => Path.GetExtension(entry.Name).Equals(".xml", StringComparison.OrdinalIgnoreCase));
                            if (!contieneArchivoXml)
                            {
                                respuesta.Codigo = 72;
                                respuesta.Resultado = false;
                                respuesta.Descripcion = $"El archivo ({part.FileName}) que se trata de procesar no contiene un archivo xml, asunto: {mensaje.Subject}";
                                log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Archivo {part.FileName} no contiene un xml", LevelMsn.Info, 0);
                                //registro error en base de datos
                                await _dataBaseHelper.RegistrarErrorsDb(infoContribuyente, log, respuesta.Codigo, mensaje.Subject);
                                return respuesta;
                            }
                            ArchivoYReceptor archivoYReceptor = new()
                            {
                                NumeroIdentificacionReceptor = infoContribuyente.NumeroIdentificacionReceptor,
                                TipoIdentificacionReceptor = infoContribuyente.TipoIdentificacionReceptor,
                                TokenEnterprise = infoContribuyente.TokenEnterprise,
                                TokenPassword = infoContribuyente.TokenPassword,
                                IdReceptor = infoContribuyente.IdReceptor,
                                FechaActualizacion = infoContribuyente.UltimaActualizacion,
                                usuario = usuario
                            };

                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                switch (Path.GetExtension(entry.FullName))
                                {
                                    case ".pdf":
                                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name, " Representación gráfica encontrada " + entry.FullName, LevelMsn.Info, 0);
                                        string representacionGrafica = CodificarDocumento(entry);
                                        archivoYReceptor.RepGraf = representacionGrafica ?? "";
                                        archivoYReceptor.NombreRg = entry.Name;
                                        break;

                                    case ".xml":
                                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name, " Archivo xml encontrado " + entry.FullName, LevelMsn.Info, 0);
                                        string ArchivoXml = CodificarDocumento(entry);
                                        archivoYReceptor.DocElectronicoRecibido = ArchivoXml;
                                        archivoYReceptor.DocElectronico = ArchivoXml;
                                        archivoYReceptor.NombreXml = entry.Name;
                                        break;

                                    case ".zip":
                                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name, " Adjunto encontrado " + entry.FullName, LevelMsn.Info, 0);
                                        string adjunto = CodificarDocumento(entry);
                                        archivoYReceptor.Adjunto = adjunto ?? "";
                                        archivoYReceptor.NombreAdjunto = entry.Name;
                                        break;
                                }
                            }
                            //**********Iniciar proceso de radicación**********//
                            log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de empezar envió documentos a radicación ", LevelMsn.Info, 0);
                            respuesta = await _multiservices.Run(archivoYReceptor, ErroresRecepcion(), log);
                        }
                        else
                        {
                            respuesta.Codigo = 73;
                            respuesta.Resultado = false;
                            respuesta.Descripcion = $"El archivo ({part.FileName}) está vacío o no contiene un archivo xml, asunto del correo: {mensaje.Subject}";
                            log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Archivo zip está vacío {part.FileName} correo asunto: {mensaje.Subject}", LevelMsn.Info, 0);
                            //registro en base de datos
                            await _dataBaseHelper.RegistrarErrorsDb(infoContribuyente, log, respuesta.Codigo, mensaje.Subject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 74;
                respuesta.Resultado = false;
                respuesta.Descripcion = $"El archivo (.zip) contiene errores, no es posible procesarlo! asunto del correo: {mensaje.Subject}";
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"El archivo (.zip) contiene errores, no es posible procesarlo! Exception= {ex.Message} asunto del mensaje: {mensaje.Subject}", LevelMsn.Info, 0);
                await _dataBaseHelper.RegistrarErrorsDb(infoContribuyente, log, respuesta.Codigo, mensaje.Subject);
            }
            return respuesta;
        }

        /// <summary>
        /// Codificar archivo a base 64 para su envio 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public string CodificarDocumento(ZipArchiveEntry entry)
        {
            string documentoCodificado;
            try
            {
                using Stream entryStream = entry.Open();
                using MemoryStream memory = new();
                entryStream.CopyTo(memory);
                byte[] doc = memory.ToArray();
                documentoCodificado = Convert.ToBase64String(doc);
            }
            catch (Exception)
            {
                documentoCodificado = "";
            }
            return documentoCodificado;
        }

        /// <summary>
        /// Errores posibles en la radicación
        /// </summary>
        /// <returns></returns>
        public List<int> ErroresRecepcion()
        {
            List<int> res = new();
            string values = _configuration.GetSection("ReceptionErrors").Value ?? "";
            string[] numbers = values.Split(',');
            foreach (string num in numbers)
            {
                res.Add(Convert.ToInt32(num));
            }
            return res;
        }
    }
}