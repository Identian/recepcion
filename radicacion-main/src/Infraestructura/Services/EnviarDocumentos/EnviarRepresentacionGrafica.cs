using CapaDominio.Interfaces.IServices.IServicesDocuments;
using CapaDominio.Radicacion;

namespace Infraestructura.Services.EnviarDocumentos
{
    public class EnviarRepresentacionGrafica : IEnviarRepresentacionGrafica
    {
        private readonly IEnviarRepresentacionGraficaProcesor _enviarRepresentacionGraficaProcesor;

        public EnviarRepresentacionGrafica(IEnviarRepresentacionGraficaProcesor enviarRepresentacionGraficaProcesor)
        {
            _enviarRepresentacionGraficaProcesor = enviarRepresentacionGraficaProcesor;
        }
        public async Task<IRespuestaRadicacion> LoadEnviarRepGrafica(string token, ArchivoYReceptor archivoYReceptor)
        {
            IRespuestaRadicacion responseGeneral = new RespuestaRadicacion();

            if (!string.IsNullOrEmpty(archivoYReceptor.RepGraf))
            {
                try
                {

                    RepresentacionGrafica representacionGrafica = new()
                    {
                        nitEmisor = archivoYReceptor.nitEmisor,
                        nombre = archivoYReceptor.NombreRg,
                        numeroDocumento = archivoYReceptor.NumDocumento,
                        TipoIdentificacionEmisor = archivoYReceptor.TipoIdentificacionEmisor,
                        TipoDocumento = archivoYReceptor.TipoDocumento
                    };
                    responseGeneral = await _enviarRepresentacionGraficaProcesor.LoadEnviarRepGrafica(token, archivoYReceptor.RepGraf, representacionGrafica);
                }
                catch (Exception ex)
                {
                    responseGeneral.codigo = 500;
                    responseGeneral.mensaje = $"Metodo LoadEnviarRepGrafica error al enviar al servicio Receptor {ex.Message}";
                    responseGeneral.resultado = "Error";
                }
            }
            else
            {
                responseGeneral.codigo = 204;
                responseGeneral.mensaje = "Se hace la carga al servicio de recepcion sin representacion gráfica";
                responseGeneral.resultado = "Informacion";
            }
            return responseGeneral;

        }
    }
}
