using Domain.Entity;
using Domain.Entity.Dto;
using Domain.Request;
using Domain.TokenContext;

namespace Domain.Interfaces
{
    public interface ISaveMetadataService
    {
        Task<IGeneralResponse> SendAsync(TransportData data);
    }
}