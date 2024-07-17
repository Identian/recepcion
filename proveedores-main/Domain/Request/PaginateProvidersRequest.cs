using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Request
{
    public class PaginateProvidersRequest
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int AplicationRoot { get; set; }
        public int AplicationUser { get; set; }
        public int IdEnterprise { get; set; }
        public string BuscarProveedor { get; set; } = null!;
        public bool OrderByDesc { get; set; } = false;
    }
}
