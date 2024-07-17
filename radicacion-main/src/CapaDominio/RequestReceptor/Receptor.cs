using CapaDominio.Interfaces.IReceptores;

namespace CapaDominio.RequestReceptor
{
    public class Receptor : IReceptor
    {
        public int IdReceptor { get; set; }
        public string? NumeroIdentificacionReceptor { get; set; }
        public string? TipoIdentificacionReceptor { get; set; }
        public string? TokenEnterprise { get; set; }
        public string? TokenPassword { get; set; }
        public int Activo { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
