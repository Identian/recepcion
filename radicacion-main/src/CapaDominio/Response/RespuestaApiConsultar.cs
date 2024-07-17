namespace CapaDominio.Response
{
    public class RespuestaApiConsultar : IRespuestaApiConsultar
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; }
        public List<IListadoCorreo> ListadoCorreos { get; set; }
        public bool Resultado { get; set; }
    }
}
