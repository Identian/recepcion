using CapaDominio.Enums.TipoAutenticacion;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;

namespace CapaDominio.Interfaces.ServiceOutlook
{
    public interface IOutlook
    {
        string access_token { get; set; }
        string client_id { get; set; }
        string client_secret { get; set; }
        DateTime expires_in { get; set; }
        string grant_type { get; set; }
        string refresh_token { get; set; }
        string scope { get; set; }
        string tenant_id { get; set; }
        TipoAutenticacion tipo_autenticacion { get; set; }
        string url_token { get; set; }
        string url_token_domain { get; set; }
        string url_token_common { get; set; }

        Task<IRespuestaApi> GuardarTokens(string usuario, ILogAzure log);
        Task<bool> Refresh_tokens(ILogAzure log);
        void SetTypeConecction(TipoAutenticacion tipoAutenticacion, string accessToken, string refreshToken, string tenantId, string correo);
    }
}