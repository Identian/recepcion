using CapaDominio.Interfaces.IServices.IServicesDocuments;
using CapaDominio.Radicacion;

namespace Infraestructura.Services.EnviarDocumentos
{
    public class EnviarXml : IEnviarXml
    {
        private readonly IEnviarXmlReceptorProcessor _enviarXMLProcessor;

        public EnviarXml(IEnviarXmlReceptorProcessor enviarXML)
        {
            _enviarXMLProcessor = enviarXML;
        }

        public async Task<IEnviarXMLResponse> LoadEnviarXML(string token, ArchivoYReceptor archivoYReceptor)
        {

            IEnviarXMLResponse enviarXML = new EnviarXMLResponse();

            if (!string.IsNullOrEmpty(archivoYReceptor.DocElectronico))
            {

                try
                {
                    enviarXML = await _enviarXMLProcessor.LoadEnviarXML(token, archivoYReceptor.DocElectronico!);
                }
                catch (Exception ex)
                {
                    enviarXML.codigo = 500;
                    enviarXML.mensaje = $"Metodo LoadEnviarXML error al enviar al servicio Receptor {ex.Message}";
                    enviarXML.resultado = "Error";
                }
            }
            else
            {
                enviarXML.codigo = 500;
                enviarXML.mensaje = "Error en la radicación del archivo";
                enviarXML.resultado = "Error";
            }
            return enviarXML;
        }


    }
}
