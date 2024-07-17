using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Responses
{
    public class GeneralResponse : IGeneralResponse
    {
        public int Codigo { get; set; }
        public string? Mensaje { get; set; }
        public string? Resultado { get; set; } 
    }
}
