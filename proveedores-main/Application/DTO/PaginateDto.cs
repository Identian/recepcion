using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class PaginateDto
    {
        public string TipoProveedor { get; set; } = null!;
        public string RazonSocial { get; set; } = null!;
        public string TipoDocumento { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public bool? Estatus { get; set; }
        public bool? Asignado { get; set; }
        public int? IdProvider { get; set; }
        public int? IdReceptor { get; set; }
        public int? AplicationUser { get; set; }
        public int? UserConfirmed { get; set; }
    }
}
