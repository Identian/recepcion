using CapaDominio.Interfaces.IServices.IServicesDocuments;
using CapaDominio.Radicacion;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Infraestructura.Services.EnviarDocumentos
{
    public class EnviarANexoProcessor : IEnviarANexoProcessor
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EnviarANexoProcessor(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IRespuestaRadicacion> LoadEnviarAnexo(string token, string archivo, dynamic anexo)
        {

            IRespuestaRadicacion ErrorResponse = new RespuestaRadicacion
            {
                codigo = 1,
                mensaje = "Sin mensaje"
            };

            try
            {
                AuthenticationHeaderValue header = new("Bearer", token);
                HttpContent data;

                var enviarAnexo = new
                {
                    anexo.numeroDocumento,
                    nombre = Path.GetFileNameWithoutExtension(anexo.nombre),
                    archivo,
                    extension = "zip",
                    visible = "1",
                    anexo.nitEmisor,
                    TipoDocumento = anexo.TipoDocumento
                };

                string json = JsonConvert.SerializeObject(enviarAnexo);
                data = new StringContent(json, Encoding.UTF8, "application/json");
                HttpClient client = _httpClientFactory.CreateClient("EnviarAnexo");
                client.DefaultRequestHeaders.Authorization = header;

                try
                {
                    HttpResponseMessage Response = await client.PostAsync("", data);
                    ErrorResponse.codigo = Convert.ToInt32(Response.StatusCode);
                    ErrorResponse.mensaje = Response.ReasonPhrase!;

                }
                catch (Exception ex)
                {
                    ErrorResponse.codigo = 500;
                    ErrorResponse.mensaje = $"Metodo LoadEnviarAnexo error enviando al servicio de recepcion {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                ErrorResponse.codigo = 500;
                ErrorResponse.mensaje = $"Metodo LoadEnviarAnexo error {ex.Message}";
            }
            return ErrorResponse;
        }
    }
}
