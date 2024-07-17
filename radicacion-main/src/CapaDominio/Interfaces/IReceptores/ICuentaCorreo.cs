using CapaDominio.Enums.TipoAutenticacion;

namespace CapaDominio.Interfaces.IReceptores
{
    public interface ICuentaCorreo
    {
        string? AccessToken { get; set; }
        string? AccessTokenExpiracionUTC { get; set; }
        bool Activo { get; set; }
        string? BandejaDescargados { get; set; }
        string? BandejaEntrada { get; set; }
        string? BandejaErroneos { get; set; }
        string? BandejaOtros { get; set; }
        string? Clave { get; set; }
        string? Estado { get; set; }
        int IdCuentaCorreo { get; set; }
        int IdCuentaCorreoReceptor { get; set; }
        short Puerto { get; set; }
        string? RefreshToken { get; set; }
        string? Servidor { get; set; }
        string? TenantID { get; set; }
        TipoAutenticacion TipoAutenticacion { get; set; }
        bool UsarSSL { get; set; }
        string? Usuario { get; set; }
    }
}