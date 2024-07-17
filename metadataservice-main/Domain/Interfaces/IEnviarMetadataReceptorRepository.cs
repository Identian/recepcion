using Domain.Request;
using Domain.TokenContext;

namespace Domain.Interfaces
{
    public interface IEnviarMetadataReceptorRepository
    {
        Task<IGeneralResponse> SaveMetadata(RequestEnviarMetadata request, CustomJwtTokenContext customJwt, ILogAzure logAzure);
    }
}