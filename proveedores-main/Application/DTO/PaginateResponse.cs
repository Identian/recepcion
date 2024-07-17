using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class PaginateResponse
    {
        public int TotalRegistros { get; set; }
        public int TotalFiltrado { get; set; }
        public List<PaginateDto> Providers { get; set; } = [];

    }
}
