using AutoMapper;
using CapaDominio.Enums.TipoAutenticacion;

namespace DTO.Automapper
{
    public class TipoAutenticacionConverter : ITypeConverter<string, TipoAutenticacion>
    {
        public TipoAutenticacion Convert(string source, TipoAutenticacion destination, ResolutionContext context)
        {
            return Enum.Parse<TipoAutenticacion>(source);
        }
    }
}
