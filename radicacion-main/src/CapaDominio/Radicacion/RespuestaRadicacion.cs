namespace CapaDominio.Radicacion
{
    public class RespuestaRadicacion : IRespuestaRadicacion
    {
        public int codigo { get; set; }
        public string? mensaje { get; set; }
        public string? resultado { get; set; }
    }
}
