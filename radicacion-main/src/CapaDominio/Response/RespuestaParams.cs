namespace CapaDominio.Response
{
    public class RespuestaParams : IRespuestaParams
    {
        public static readonly string SaltosDeLinea = Environment.NewLine + Environment.NewLine;
        public int IdEntrada { get; set; }
        public int IdSalida { get; set; }
        public string? Metodo { get; set; }
        public bool Resultado { get; set; }
        public int Codigo { get; set; }
        public string? ValorString { get; set; }
        public List<object> ValorObject { get; set; } = new List<object>();
        public string? Descripcion { get; set; }
        public string? Detalles { get; set; }
        public string? DetallesAdicionales { get; set; }
        public int MensajesProcesados { get; set; }
        public bool TerminarProceso { get; set; }
    }
}