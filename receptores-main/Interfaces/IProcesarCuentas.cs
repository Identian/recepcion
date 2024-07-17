using Receptores.Model.Estructuras;

namespace Receptores.Interfaces
{
    public interface IProcesarCuentas
    {
        Task ProcesarMensajes(Estructura cuenta);
    }
}