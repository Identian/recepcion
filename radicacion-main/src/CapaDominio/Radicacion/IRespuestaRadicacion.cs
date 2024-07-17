namespace CapaDominio.Radicacion
{
    public interface IRespuestaRadicacion
    {
        int codigo { get; set; }
        string mensaje { get; set; }
        string resultado { get; set; }
    }
}