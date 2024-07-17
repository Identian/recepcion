using CapaDominio.Auth;
using CapaDominio.Interfaces.IHelpers;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using Infraestructura.Logs;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Infraestructura.Repository
{
    public class SaveTenantRepository : ISaveTenantRepository
    {
        private readonly IRespuestaApi _respuestaApi;
        private readonly IObjectConversion<State> _objectConversion;
        private readonly IProcesoAuthentication _procesoAuthentication;
        private readonly IConfiguration _configuration;

        public SaveTenantRepository(IRespuestaApi respuestaApi, IObjectConversion<State> objectConversion, IProcesoAuthentication procesoAuthentication, IConfiguration configuration)
        {
            _respuestaApi = respuestaApi;
            _objectConversion = objectConversion;
            _procesoAuthentication = procesoAuthentication;
            _configuration = configuration;
        }

        public async Task<IRespuestaApi> SaveTenant(Authenticate authenticate)
        {
            ILogAzure logAzure = new LogAzure(_configuration, "0");
            _respuestaApi.Codigo = 500;
            _respuestaApi.Mensaje = "Error desconocido. ";

            try
            {
                string cleanState = Uri.UnescapeDataString(authenticate.State!).TrimEnd('#');
                byte[] byteState = Convert.FromBase64String(cleanState);
                string stateFinal = Encoding.UTF8.GetString(byteState);

                State StateObj = _objectConversion.FromJson(stateFinal);
                IRespuesta guardarResultado = await _procesoAuthentication.GuardarTenantID(StateObj.Id_receptor, StateObj.Email!, authenticate.TenantId!, logAzure);

                if (guardarResultado.Resultado)
                {
                    _respuestaApi.Codigo = 200;
                    _respuestaApi.Mensaje = guardarResultado.Descripcion!;
                }
                else
                {
                    _respuestaApi.Codigo = 500;
                    _respuestaApi.Mensaje = guardarResultado.Descripcion!;
                }

                return _respuestaApi;
            }
            catch (Exception ex)
            {
                _respuestaApi.Mensaje += ex.Message;
                return _respuestaApi;
            }
        }
    }
}
