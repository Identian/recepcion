using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Request
{
    /// <summary>
    /// Clase representación del request que llega al controlador
    /// </summary>
    public class RequestEnviarMetadata
    {
        public string NitEmisor { get; set; } = null!;
        public string TipoIdentificacionEmisor { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public List<MetadataRequest> Metadata { get; set; } = [];
        public string TipoDocumento { get; set; } = null!;
    }
}
