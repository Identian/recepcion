namespace CapaDominio.Radicacion
{
    public interface IEnviarXMLResponse
    {
        int codigo { get; set; }
        string? documentoId { get; set; }
        string? mensaje { get; set; }
        string? numeroIdentificacion { get; set; }
        string? resultado { get; set; }
        string? tipoIdentificacion { get; set; }
        string? TipoDocumento { get; set; }
    }
}