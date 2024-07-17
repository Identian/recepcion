using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IServices;
using Newtonsoft.Json;

namespace Infraestructura.Services.ConsultaApi
{
    public class ConsultarCuentaCorreoReceptorDesdeApi : IConsultarCuentaCorreoReceptorDesdeApi
    {
        public async Task<ICuentaCorreo> ConsultarCuentaCorreoReceptor(int idCuentaCorreoReceptor)
        {
            string url = "https://localhost:44342/API/CuentaCorreoReceptor";
            HttpClient httpClient = new HttpClient();
            string jsonConsulta = await httpClient.GetStringAsync(url);
            List<ICuentaCorreo> receptores = JsonConvert.DeserializeObject<List<ICuentaCorreo>>(jsonConsulta);
            ICuentaCorreo CuentaCorreoReceptor = receptores.Single(i => i.IdCuentaCorreo == idCuentaCorreoReceptor);

            return CuentaCorreoReceptor;

        }
    }
}
