namespace Receptores.Model.Receptores
{
    public class Receptor
    {
        public string? IdReceptor { get; set; }
        public string? NumeroIdentificacionReceptor { get; set; }
        public string? TipoIdentificacionReceptor { get; set; }
        public string? TokenEnterprise { get; set; }
        public string? TokenPassword { get; set; }
        public string? ServicioRecepcion { get; set; }
        public bool Activo { set; get; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int IdCuentaCorreoReceptor { get; set; }
        public int IdCuentaCorreo { set; get; }
        public string? Usuario { get; set; }
        public string? Servidor { get; set; }
        public string? Puerto { get; set; }
        public bool? UsarSSL { get; set; }
        public string? Clave { set; get; }
        public string? BandejaEntrada { get; set; }
        public string? BandejaDescargados { get; set; }
        public string? BandejaErroneos { get; set; }
        public string? BandejaOtros { get; set; }
        public string? AccessToken { get; set; }
        public DateTime AccessTokenExpiracionUTC { get; set; }
        public string? RefreshToken { get; set; }
        public string? TipoAutenticacion { get; set; }
        public string? TenantID { get; set; }
        public bool Estado { get; set; }
        public int Task_status { get; set; }
        public DateTime Date_status { get; set; }
    }
}
