using CapaDominio.Interfaces.IServices.IServicesDocuments;
using CapaDominio.Radicacion;

namespace Infraestructura.Services.EnviarDocumentos
{
    public class EnviarAnexo : IEnviarAnexo
    {
        private readonly IEnviarANexoProcessor _enviarANexoProcessor;

        public EnviarAnexo(IEnviarANexoProcessor enviarANexoProcessor)
        {
            _enviarANexoProcessor = enviarANexoProcessor;
        }
        public async Task<IRespuestaRadicacion> LoadEnviarAnexo(string token, ArchivoYReceptor archivoYReceptor)
        {
            IRespuestaRadicacion respuestaRadicacion = new RespuestaRadicacion();
            if (!string.IsNullOrEmpty(archivoYReceptor.Adjunto))
            {
                try
                {
                    var datosAnexo = new
                    {
                        archivoYReceptor.nitEmisor,
                        nombre = archivoYReceptor.NombreAdjunto,
                        numeroDocumento = archivoYReceptor.NumDocumento,
                        TipoDocumento = archivoYReceptor.TipoDocumento!
                    };

                    respuestaRadicacion = await _enviarANexoProcessor.LoadEnviarAnexo(token, archivoYReceptor.Adjunto, datosAnexo);
                }
                catch (Exception ex)
                {
                    respuestaRadicacion.codigo = 500;
                    respuestaRadicacion.mensaje = $"Metodo LoadEnviarAnexo error al enviar al servicio Receptor {ex.Message}";
                    respuestaRadicacion.resultado = "Error";
                }
            }
            else
            {
                respuestaRadicacion.codigo = 204;
                respuestaRadicacion.mensaje = "Se hace la carga al servicio de recepcion sin adjuntos";
                respuestaRadicacion.resultado = "Informacion";
            }
            return respuestaRadicacion;
        }
    }
}
