using CapaDominio.RequestReceptor;

namespace Infraestructura.Services.DocumentosElectronico
{
    public class MensajeBase
    {
        public int IdEntrada { get; set; }
        public int IdEjecucion { get; set; }
        public uint IdUnicoImap { get; set; }
        public string Usuario { get; set; }
        public List<string> Destinatarios { get; set; }
        public string Remitente { get; set; }
        public string Asunto { get; set; }
        public DateTimeOffset FechaHora { get; set; }
        public string CarpetaOrigen { get; set; }
        public string CarpetaDestino { get; set; }
        public short IdTipoMensaje { get; set; }
        public DocumentoElectronicoBase DocumentoElectronicoXML { get; set; }
        public ArchivoBase RepresentacionGraficaPDF { get; set; }
        public List<IArchivoBase> Adjuntos { get; set; }

        public string GetParametrosSQL()
        {
            return

              nameof(IdEntrada) + "=" + IdEntrada.ToString() + ";" +
              nameof(IdEjecucion) + "=" + IdEjecucion.ToString() + ";" +
              nameof(IdUnicoImap) + "=" + IdUnicoImap.ToString() + ";" +
              nameof(Usuario) + "=" + Usuario + ";" +
              nameof(Remitente) + "=" + Remitente + ";" +
              nameof(FechaHora) + "=" + FechaHora.ToString() + ";" +
              nameof(CarpetaOrigen) + "=" + CarpetaOrigen + ";" +
              nameof(CarpetaDestino) + "=" + CarpetaDestino + ";" +
              nameof(IdTipoMensaje) + "=" + IdTipoMensaje.ToString()
            ;
        }

        public void Clear()
        {
            IdEntrada = 0;
            IdEjecucion = 0;
            IdUnicoImap = 0;
            Usuario = "";
            Destinatarios = new List<string>();
            Destinatarios.Clear();
            Remitente = "";
            Asunto = "";
            FechaHora = DateTimeOffset.MinValue;
            CarpetaOrigen = "";
            CarpetaDestino = "";
            IdTipoMensaje = 0;
            DocumentoElectronicoXML = new DocumentoElectronicoBase();
            RepresentacionGraficaPDF = new ArchivoBase();
            Adjuntos = new List<IArchivoBase>();
            Adjuntos.Clear();
        }

        private MensajeBase() { }

        public MensajeBase(int varIdEjecucion)
        {
            IdEjecucion = varIdEjecucion;
            Clear();
            IdEjecucion = varIdEjecucion;
        }
    }
}
