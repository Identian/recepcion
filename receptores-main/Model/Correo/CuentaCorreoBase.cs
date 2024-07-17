namespace Receptores.Model.Correo
{
    public class CuentaCorreoReceptorBase
    {
        public int IdCuentaCorreoReceptor { get; set; }
        public int IdCuentaCorreo { get; set; }
        public string? Usuario { get; set; }
        public string? Servidor { get; set; }
        public short Puerto { get; set; }
        public bool UsarSSL { get; set; }
        public string? Clave { get; set; }
        public string? BandejaEntrada { get; set; }
        public string? BandejaDescargados { get; set; }
        public string? BandejaErroneos { get; set; }
        public string? BandejaOtros { get; set; }
        public string? AccessToken { get; set; }
        public DateTime? AccessTokenExpiracionUTC { get; set; }
        public string? RefreshToken { get; set; }
        public string? TipoAutenticacion { get; set; }
        public string? TenantID { get; set; }
        public string? Estado { get; set; }
        public bool Activo { get; set; }
    }
}
