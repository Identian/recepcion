namespace CapaDominio.Response
{
    public class RespuestaApi : IRespuestaApi
    {
        public int Codigo { get; set; }
        public string? Mensaje { get; set; }
    }
}
