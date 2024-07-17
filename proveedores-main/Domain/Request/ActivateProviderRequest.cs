using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Request
{
    public class ActivateProviderRequest
    {
        public int IdReceptor { get; set; }
        public int IdProveedor { get; set; }
    }
}
