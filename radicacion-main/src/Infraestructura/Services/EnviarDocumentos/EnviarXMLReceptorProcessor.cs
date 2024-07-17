using CapaDominio.Interfaces.IServices.IServicesDocuments;
using CapaDominio.Radicacion;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Infraestructura.Services.EnviarDocumentos
{
    public class EnviarXmlReceptorProcessor : IEnviarXmlReceptorProcessor
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EnviarXmlReceptorProcessor(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnviarXMLResponse> LoadEnviarXML(string token, string archivo)
        {
            EnviarXMLResponse? ErrorResponse = new()
            {
                codigo = 1,
                mensaje = "Sin mensaje"
            };

            try
            {
                AuthenticationHeaderValue header = new("Bearer", token);
                HttpContent data;

                string json = JsonConvert.SerializeObject(new { archivo });
                data = new StringContent(json, Encoding.UTF8, "application/json");

                HttpClient client = _httpClientFactory.CreateClient("EnviarXML");
                client.DefaultRequestHeaders.Authorization = header;
                try
                {
                    HttpResponseMessage Response = await client.PostAsync("", data);
                    ErrorResponse = await Response.Content.ReadFromJsonAsync<EnviarXMLResponse>();
                }
                catch (TaskCanceledException ex)
                {
                    ErrorResponse!.codigo = 500;
                    ErrorResponse.mensaje = $"Metodo EnviarXMLReceptorProcessor.LoadEnviarXML Error al enviar al servicio Receptor {ex.Message}";
                }
            }
            catch (Exception ms)
            {
                ErrorResponse!.codigo = 500;
                ErrorResponse.mensaje = $"Metodo EnviarXMLReceptorProcessor.LoadEnviarXML Error: {ms.Message}";
            }
            return ErrorResponse!;
        }
    }
}
