using Domain.Request;
using Domain.TokenContext;

namespace Domain.Interfaces
{
    public interface IEnviarMetadataEmisorRepository
    {
        Task<IGeneralResponse> SendMetadataEmisor(RequestEnviarMetadata request, CustomJwtTokenContext customJwt, ILogAzure logAzure);
    }
}