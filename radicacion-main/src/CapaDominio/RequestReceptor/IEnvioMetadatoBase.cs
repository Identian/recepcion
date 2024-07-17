namespace CapaDominio.RequestReceptor
{
    public interface IEnvioMetadatoBase
    {
        string Codigo { get; set; }
        string ConsecutivoDocumento { get; set; }
        bool Exitoso { get; set; }
        int IdDocumento { get; set; }
        int IdEjecucion { get; set; }
        int IdEntrada { get; set; }
        int IdSalida { get; set; }
        string NumeroIdentificacionEmisor { get; set; }
        string TipoIdentificacionEmisor { get; set; }
        string Valor { get; set; }

        void Clear();
    }
}