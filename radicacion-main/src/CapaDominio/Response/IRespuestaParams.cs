namespace CapaDominio.Response
{
    public interface IRespuestaParams
    {
        int Codigo { get; set; }
        string? Descripcion { get; set; }
        string? Detalles { get; set; }
        string? DetallesAdicionales { get; set; }
        int IdEntrada { get; set; }
        int IdSalida { get; set; }
        string? Metodo { get; set; }
        bool Resultado { get; set; }
        int MensajesProcesados { get; set; }
        List<object> ValorObject { get; set; }
        string? ValorString { get; set; }
        bool TerminarProceso { get; set; }
    }
}