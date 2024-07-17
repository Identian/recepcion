using CapaDominio.Enums.TipoAutenticacion;

namespace CapaDominio.Response
{
    public interface IListadoCorreo
    {
        string correo { get; set; }
        string puerto { get; set; }
        string servidor { get; set; }
        TipoAutenticacion tipoAutenticacion { get; set; }
        bool usarSSL { get; set; }
    }
}