namespace CapaDominio.RequestReceptor
{
    public class EnvioMetadatoBase : IEnvioMetadatoBase
    {
        public int IdEntrada { get; set; }
        public int IdSalida { get; set; }
        public int IdEjecucion { get; set; }
        public int IdDocumento { get; set; }
        public string NumeroIdentificacionEmisor { get; set; }
        public string TipoIdentificacionEmisor { get; set; }
        public string ConsecutivoDocumento { get; set; }
        public string Codigo { get; set; }
        public string Valor { get; set; }
        public bool Exitoso { get; set; }

        public void Clear()
        {
            IdEntrada = 0;
            IdSalida = 0;
            IdEjecucion = 0;
            IdDocumento = 0;
            NumeroIdentificacionEmisor = "";
            TipoIdentificacionEmisor = "";
            ConsecutivoDocumento = "";
            Codigo = "";
            Valor = "";
            Exitoso = false;
        }

        private EnvioMetadatoBase() { }

        public EnvioMetadatoBase(int varIdEjecucion)
        {
            Clear();
            IdEjecucion = varIdEjecucion;
        }
    }
}
