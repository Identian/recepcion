using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Invoice;
using CapaDominio.RequestReceptor;
using CapaDominio.Response;

namespace CapaDominio.Interfaces.IDB
{
    public interface IDataBase
    {
        Task<IRespuesta> ActualizarTokensOauth(string usuario, string access_token, string refresh_token, DateTime expires, ILogAzure log);
        Task Conexion();
        Task<IRespuestaApiConsultar> ConsultarEmail(string numeroIdentificacion, string tipoIdentificacion, ILogAzure log);
        Task<IRespuesta> ConsultarTipoAutenticacion(int id_receptor, string email, ILogAzure log);
        Task<IRespuesta> InactivarCorreoReceptor(string Nit, string TipoIdentificacion, ILogAzure log);
        Task<ICuentaCorreo> ObtenerCredencialesOauth(int id_receptor, string email, ILogAzure log);
        Task<IRespuesta> RegistrarCuentaCorreoReceptor(CuentaCorreoGuardar parametros, ILogAzure log);
        Task<IRespuesta> RegistrarInvoiceReceptionErrors(InvoiceReceptionError parametros, ILogAzure log);
        Task<IRespuesta> RegistrarTenantID(int id_receptor, string email, string tenantID, ILogAzure log);

    }
}