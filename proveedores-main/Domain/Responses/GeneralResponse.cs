using Domain.Interfaces;

namespace Domain.Responses
{
    public class GeneralResponse : IGeneralResponse
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; } = null!;
        public string Resultado { get; set; } = null!; 
    }
}
