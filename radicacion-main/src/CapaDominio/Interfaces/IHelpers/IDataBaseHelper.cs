using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Response;

namespace CapaDominio.Interfaces.IHelpers
{
    public interface IDataBaseHelper
    {
        Task<IRespuesta> RegistrarErrorsDb(IReceptorBase receptorBase, ILogAzure logAzure, int codigo, string subject);
    }
}
