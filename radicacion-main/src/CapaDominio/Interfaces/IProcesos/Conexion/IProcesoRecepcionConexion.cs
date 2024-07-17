using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;

namespace CapaDominio.Interfaces.IProcesos.Conexion
{
    public interface IProcesoRecepcionConexion
    {
        IRespuesta Conectar(string servidor, int puerto, bool usarSsl, string usuario, string clave, ILogAzure log);
    }
}