namespace CapaDominio.RequestReceptor
{
    public interface IArchivoBase
    {
        string? Descargado { get; set; }
        string? DirectorioDestino { get; set; }
        string? DirectorioOrigen { get; set; }
        string? EnviarComo { get; set; }
        string? Guardado { get; set; }
        int IdEntrada { get; set; }
        int IdMensaje { get; set; }
        int IdSalida { get; set; }
        short IdTipoArchivo { get; set; }
        string? Recibido { get; set; }

        string GetDirectorioOrigenFinal();
    }
}