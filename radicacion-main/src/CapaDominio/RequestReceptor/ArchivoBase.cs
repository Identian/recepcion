namespace CapaDominio.RequestReceptor
{
    public class ArchivoBase : IArchivoBase
    {
        public int IdEntrada { get; set; }
        public int IdSalida { get; set; }
        public short IdTipoArchivo { get; set; }
        public int IdMensaje { get; set; }
        public string? DirectorioOrigen { get; set; }
        public virtual string GetDirectorioOrigenFinal()
        {
            return DirectorioOrigen!;
        }
        public string? Recibido { get; set; }
        public string? Descargado { get; set; }
        public string? Guardado { get; set; }
        public string? DirectorioDestino { get; set; }
        public string? EnviarComo { get; set; }
    }
}
