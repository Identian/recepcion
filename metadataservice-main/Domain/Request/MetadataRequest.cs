using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Request
{
    /// <summary>
    /// Clase metadata interna
    /// </summary>
    public class MetadataRequest
    {
        public int Code { get; set; }
        public string Value { get; set; } = null!;
        public string Internal1 { get; set; } = null!;
        public string Internal2 { get; set; } = null!;
        public string Id { get; set; } = null!;
    }
}
