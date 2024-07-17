using Receptores.Model.Estructuras;

namespace Receptores.Interfaces
{
    public interface IDatabaseQueries
    {
        Task<List<Estructura>> GetAccounts();
        Task<int> UpdateAccounts(int status, int IdCuentaCorreoReceptor);

        Task<bool> UpdateAcountsFree();
    }
}