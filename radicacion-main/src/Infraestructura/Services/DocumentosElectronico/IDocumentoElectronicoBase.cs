using CapaDominio.RequestReceptor;

namespace Infraestructura.Services.DocumentosElectronico
{
    public interface IDocumentoElectronicoBase
    {
        string? Consecutivo { get; set; }
        int IdEmisor { get; set; }
        List<IEnvioMetadatoBase> ListaMetadatos { get; set; }
        string? NumeroIdentificacionEmisor { get; set; }
        string? NumeroIdentificacionReceptor { get; set; }
        string? TipoIdentificacionEmisor { get; set; }
        string? TipoIdentificacionReceptor { get; set; }
        string? UUID { get; set; }
    }
}