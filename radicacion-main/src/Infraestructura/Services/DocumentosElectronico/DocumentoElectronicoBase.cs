using CapaDominio.RequestReceptor;

namespace Infraestructura.Services.DocumentosElectronico
{
    public class DocumentoElectronicoBase : ArchivoBase, IDocumentoElectronicoBase
    {
        public int IdEmisor { get; set; }
        public string? Consecutivo { get; set; }
        public string? UUID { get; set; }
        public string? NumeroIdentificacionEmisor { get; set; }
        public string? TipoIdentificacionEmisor { get; set; }
        public string? NumeroIdentificacionReceptor { get; set; }
        public string? TipoIdentificacionReceptor { get; set; }
        public List<IEnvioMetadatoBase> ListaMetadatos { get; set; } = new List<IEnvioMetadatoBase>();
    }
}
