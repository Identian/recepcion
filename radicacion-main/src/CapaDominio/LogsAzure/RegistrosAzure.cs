using CapaDominio.Interfaces.LogsAzure;
using System.Diagnostics;

namespace CapaDominio.LogsAzure
{
    public class RegistrosAzure
    {
        public ILogAzure _logAzure { get; set; }
        public string? Codigo { get; set; }
        public string? NumeroIdentificacionReceptor { get; set; }
        public string? Descripcion { get; set; }

        public string? Usuario { get; set; }
        public Stopwatch time { get; set; }
    }
}
