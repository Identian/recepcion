namespace DTO.RegistrarEmailDto
{
    public class GuardarEmailDto
    {
        public string? NIT { get; set; }
        public string? Servidor { get; set; }
        public string? Puerto { get; set; }
        public bool UsarSSL { get; set; }
        public string? Usuario { get; set; }
        public string? Clave { get; set; }
        public string? TipoIdentificadorReceptor { get; set; }
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }
        public string? expires_in { get; set; }
        public string? ext_expires_in { get; set; }
        public string? TipoAutenticacion { get; set; }
    }
}
