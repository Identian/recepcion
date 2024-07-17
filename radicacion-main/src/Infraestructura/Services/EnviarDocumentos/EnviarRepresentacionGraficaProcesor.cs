using CapaDominio.Interfaces.IServices.IServicesDocuments;
using CapaDominio.Radicacion;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Infraestructura.Services.EnviarDocumentos
{
    public class EnviarRepresentacionGraficaProcesor : IEnviarRepresentacionGraficaProcesor
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EnviarRepresentacionGraficaProcesor(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IRespuestaRadicacion> LoadEnviarRepGrafica(string token, string archivo, RepresentacionGrafica repGrafica)
        {
            IRespuestaRadicacion? ErrorResponse = new RespuestaRadicacion()
            {
                codigo = 1,
                mensaje = "Sin mensaje"
            };
            try
            {
                AuthenticationHeaderValue header = new("Bearer", token);
                HttpContent data;

                var datos = new
                {
                    repGrafica.nitEmisor,
                    repGrafica.numeroDocumento,
                    nombre = Path.GetFileNameWithoutExtension(repGrafica.nombre),
                    archivo,
                    extension = "pdf",
                    visible = "1",
                    tipoIdentificacionemisor = repGrafica.TipoIdentificacionEmisor,
                    TipoDocumento = repGrafica.TipoDocumento!
                };
                string json = JsonConvert.SerializeObject(datos);
                data = new StringContent(json, Encoding.UTF8, "application/json");
                //implementación en la cap de injección de dependencias
                using HttpClient client = _httpClientFactory.CreateClient("EnviarRepGrafica");
                client.DefaultRequestHeaders.Authorization = header;
                try
                {
                    using HttpResponseMessage Response = await client.PostAsync("", data);
                    ErrorResponse = await Response.Content.ReadFromJsonAsync<RespuestaRadicacion>();
                }
                catch (Exception ex)
                {
                    ErrorResponse!.codigo = 500;
                    ErrorResponse.mensaje = $"Metodo LoadEnviarRepGrafica error enviando al servicio de recepcion {ex.Message}";
                }
            }
            catch (Exception ex)
            {
                ErrorResponse!.codigo = 500;
                ErrorResponse.mensaje = $"Metodo LoadEnviarRepGrafica Error {ex.Message}";
            }
            return ErrorResponse!;
        }
    }
}
