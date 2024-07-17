using CapaDominio.Interfaces.IReceptores;

namespace CapaDominio.Receptor
{
    public class ReceptorBase : IReceptorBase
    {
        public int IdReceptor { get; set; }
        public string? NumeroIdentificacionReceptor { get; set; }
        public string? ServicioRecepcion { get; set; }
        public string? TipoIdentificacionReceptor { get; set; }
        public string? TokenEnterprise { get; set; }
        public string? TokenPassword { get; set; }
        public DateTime UltimaActualizacion { get; set; }

        public void Clear()
        {
            IdReceptor = 0;
            NumeroIdentificacionReceptor = "";
            TipoIdentificacionReceptor = "";
            TokenEnterprise = "";
            TokenPassword = "";
            ServicioRecepcion = "";
            UltimaActualizacion = DateTime.Today;
        }

        public ReceptorBase()
        {
            Clear();
        }
    }
}
