using CapaDominio.Enums.TipoAutenticacion;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.RequestReceptor;
using CapaDominio.Response;

namespace CapaDominio.Interfaces.IServices
{
    public interface IProcesoGestionEmail
    {
        Task<IRespuestaApiConsultar> ConsultarEmail(string numeroIdentificacion, string tipoIdentificacion, ILogAzure log);
        Task<TipoAutenticacion> ConsultarTipoAutenticacion(int id_receptor, string email, ILogAzure log);
        Task<IRespuestaApi> InactivarCorreoReceptor(string Nit, string TipoIdentificacion, ILogAzure log);
        Task<IRespuestaApi> RegistrarActualizarEmail(CuentaCorreoGuardar parametros, ILogAzure log);
    }
}