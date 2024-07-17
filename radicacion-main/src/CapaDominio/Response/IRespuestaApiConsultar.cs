namespace CapaDominio.Response
{
    public interface IRespuestaApiConsultar
    {
        int Codigo { get; set; }
        List<IListadoCorreo> ListadoCorreos { get; set; }
        string Mensaje { get; set; }
        bool Resultado { get; set; }
    }
}