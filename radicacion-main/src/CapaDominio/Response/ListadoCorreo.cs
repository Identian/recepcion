using CapaDominio.Enums.TipoAutenticacion;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CapaDominio.Response
{
    public class ListadoCorreo : IListadoCorreo
    {
        public string? correo { get; set; }
        public string? servidor { get; set; }
        public string? puerto { get; set; }
        public bool usarSSL { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TipoAutenticacion tipoAutenticacion { get; set; }
    }
}
