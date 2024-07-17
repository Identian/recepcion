using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IServices;
using Newtonsoft.Json;

namespace Infraestructura.Services.ConsultarReceptoresApi
{
    public class ConsultarReceptorDesdeApi : IConsultarReceptorDesdeApi
    {
        public async Task<IReceptor> ConsultaReceptor(int idReceptor)
        {
            HttpClient httpClient = new();
            string jsonConsulta = await httpClient.GetStringAsync("https://localhost:44342/API/Receptor");
            List<IReceptor> receptores = JsonConvert.DeserializeObject<List<IReceptor>>(jsonConsulta);
            IReceptor receptor = receptores.Single(i => i.IdReceptor == idReceptor);

            return receptor;

        }
    }
}
