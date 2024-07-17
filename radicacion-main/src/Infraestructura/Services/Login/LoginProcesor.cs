using CapaDominio.Auth;
using CapaDominio.Interfaces.IAuth;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace Infraestructura.Services.Login
{
    public class LoginProcesor : ILoginProcesor
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public LoginProcesor(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ILoginResponse> Login(ILoginSoap login)
        {
            try
            {
                string json = JsonConvert.SerializeObject(login);
                StringContent data = new(json, Encoding.UTF8, "application/json");
                HttpClient httpClient = _httpClientFactory.CreateClient("Login");
                HttpResponseMessage respuesta = await httpClient.PostAsync("", data);
                if (respuesta.IsSuccessStatusCode)
                {
                    LoginResponse? loginResponse = await respuesta.Content.ReadFromJsonAsync<LoginResponse>();
                    return loginResponse!;
                }
                else
                {
                    return new LoginResponse();
                }
            }
            catch (Exception)
            {
                return new LoginResponse();
            }
        }
    }
}

