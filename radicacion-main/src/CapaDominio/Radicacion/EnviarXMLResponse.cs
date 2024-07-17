namespace CapaDominio.Radicacion
{
    public class EnviarXMLResponse : IEnviarXMLResponse
    {
        public int codigo { get; set; }
        public string? mensaje { get; set; }
        public string? resultado { get; set; }

        public string? documentoId { get; set; }
        public string? numeroIdentificacion { get; set; }
        public string? tipoIdentificacion { get; set; }
        public string? TipoDocumento { get; set; }
    }
}
