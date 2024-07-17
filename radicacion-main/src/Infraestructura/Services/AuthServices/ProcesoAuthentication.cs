using CapaDominio.Interfaces.IDB;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;
using MailKit.Net.Imap;

namespace Infraestructura.Services.AuthServices
{
    public class ProcesoAuthentication : IProcesoAuthentication
    {
        private readonly IDataBase _dataBase;
        private readonly IEmailConectar _emailConectar;

        public ProcesoAuthentication(IDataBase dataBase, IEmailConectar emailConectar)
        {
            _dataBase = dataBase;
            _emailConectar = emailConectar;
        }
        public async Task<IRespuesta> GuardarTenantID(int id_receptor, string email, string tenantID, ILogAzure log)
        {
            IRespuesta respuesta = await _dataBase.RegistrarTenantID(id_receptor, email, tenantID, log);
            return respuesta;
        }

        public async Task<IRespuesta> VerificarAccesos(int id_receptor, string email, ILogAzure log)
        {
            IRespuesta result = new Respuesta();
            ICuentaCorreo credenciales = await _dataBase.ObtenerCredencialesOauth(id_receptor, email, log);

            if (credenciales != null)
            {
                credenciales.Usuario = email;
                result = await _emailConectar.Conectar(credenciales, log);

                //remover el cliente
                result.ValorObject.RemoveAt(0);
            }
            else
            {
                result.Codigo = 500;
                result.Descripcion = "No se consiguieron las credenciales para el correo " + email;
            }
            return result;
        }
    }
}
