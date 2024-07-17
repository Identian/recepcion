using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGeneralResponse
    {
        int Codigo { get; set; }
        string Mensaje { get; set; }
        string Resultado { get; set; }
    }
}
