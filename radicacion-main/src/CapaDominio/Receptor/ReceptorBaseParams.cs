using CapaDominio.Interfaces.IReceptores;

namespace CapaDominio.Receptor
{
    public class ReceptorBaseParams : IReceptorBaseParams
    {
        public int IdReceptor { get; set; }
        public string? NumeroIdentificacionReceptor { get; set; }
        public string? TipoIdentificacionReceptor { get; set; }
        public string? TokenEnterprise { get; set; }
        public string? TokenPassword { get; set; }
        public string? ServicioRecepcion { get; set; }
        public DateTime UltimaActualizacion { get; set; }
    }
}
