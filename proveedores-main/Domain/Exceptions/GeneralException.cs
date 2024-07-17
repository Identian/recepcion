using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    /// <summary>
    /// Excepción perzonalizada, esto permite usar presentadores en la capa de Application
    /// </summary>
    public class GeneralException : Exception
    {
        public string? Details { get; set; }
        public GeneralException(string message, Exception exception) : base(message, exception) { }
        public GeneralException(string message) : base(message) { }
        public GeneralException() { }
        public GeneralException(string title, string detail) : base() { 
            Details = detail;
        }

    }
}
