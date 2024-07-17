namespace DTO.EmailPing
{
    public class EmailPingDto
    {
        public string? Usuario { get; set; }
        public string? Servidor { get; set; }
        public string? Puerto { get; set; }
        public bool UsarSSL { get; set; }
        public string? Clave { get; set; }
    }
}
