using CapaDominio.Enums.Logs;
using CapaDominio.Interfaces.IHelpers;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Reflection;

namespace Infraestructura.Services.RecepciónDocumentosZip
{
    public class ProcesoRecepcionDocumentoZip : IProcesoRecepcionDocumentoZip
    {
        private readonly IConfiguration _configuration;
        private readonly IRadicacion _radicacion;
        private readonly IRespuesta _respuesta;
        private readonly IDataBaseHelper _dataBaseHelper;
        public ProcesoRecepcionDocumentoZip(IConfiguration configuration, IRespuesta respuesta, IRadicacion radicacion, IDataBaseHelper dataBaseHelper)
        {
            _configuration = configuration;
            _respuesta = respuesta;
            _radicacion = radicacion;
            _dataBaseHelper = dataBaseHelper;
        }

        private int ContarDocumentosZipRecibidos(MimeMessage mensaje)
        {
            string extensiónZip = _configuration.GetSection("Extension:DocumentoZip").Value ?? ".zip";
            int result = 0;
            IEnumerable<MimePart> attachments = mensaje.BodyParts.OfType<MimePart>().Where(part => !string.IsNullOrEmpty(part.FileName));
            try
            {
                foreach (MimePart attachment in attachments)
                {
                    if (Path.GetExtension(attachment.FileName).ToLower() == extensiónZip.ToLower())
                    {
                        result++;
                    }
                }
            }
            catch (Exception)
            {
                //
            }
            return result;
        }


        public async Task<IRespuesta> RadicarZip(MimeMessage mensaje, IReceptorBase infoContribuyente, ILogAzure log, string usuario)
        {
            IRespuesta result = _respuesta;
            result.SetMetodo($"{nameof(ProcesoRecepcionDocumentoZip)} . DescargarDocumentoZip");

            int zipRecibidos = ContarDocumentosZipRecibidos(mensaje);

            log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Archivos (.zip) encontrados={zipRecibidos}", LevelMsn.Info, 0);

            if (zipRecibidos == 1)
            {
                try
                {
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de empezar proceso de radicación documentos ", LevelMsn.Info, 0);
                    result = await _radicacion.RadicarArchivoAsync(mensaje, infoContribuyente, log, usuario);
                }
                catch (Exception ex)
                {
                    result.Codigo = 500;
                    result.Resultado = false;
                    result.Descripcion = "Se ha producido un error en la descarga la carpeta comprimida";
                    result.DetallesAdicionales = "Archivo adjunto en el correo no puede ser procesado";
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(ex), LevelMsn.Error, 0);
                }
            }
            else
            {
                if (zipRecibidos == 0)
                {
                    result.Codigo = 71;
                    result.Resultado = false;
                    result.Descripcion = $"No se encontró documento comprimido (.ZIP) en el mensaje de correo. asunto del correo: {mensaje.Subject}";
                    result.DetallesAdicionales = "Correo no tiene Archivo ZIP";
                    await _dataBaseHelper.RegistrarErrorsDb(infoContribuyente, log, result.Codigo, mensaje.Subject);
                }
                else
                {
                    result.Codigo = 70;
                    result.Resultado = false;
                    result.Descripcion = $"Se encontró más de un documento electrónico en el mensaje de correo. asunto del correo: {mensaje.Subject}";
                    result.DetallesAdicionales = "Correo con mas de un Archivo ZIP";

                    await _dataBaseHelper.RegistrarErrorsDb(infoContribuyente, log, result.Codigo, mensaje.Subject);

                }
            }
            return result;
        }
    }
}
