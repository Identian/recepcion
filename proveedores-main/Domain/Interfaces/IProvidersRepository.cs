using Domain.Entity.Store_Procedure;
using Domain.Request;

namespace Domain.Interfaces
{
    public interface IProvidersRepository
    {
        Task<IGeneralResponse> ActivateAsync(ActivateProviderRequest request, ILogAzure logAzure);
        Task<List<SpPaginateProviders>> PaginateProviders(PaginateProvidersRequest request, ILogAzure logAzure);
        Task<IGeneralResponse> LinkUsersProvidersAsync(UsuariosProveedoresRequest proveedoresRequest, ILogAzure logAzure);
    }
}