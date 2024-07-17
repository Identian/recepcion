using Application.DTO;
using Domain.Interfaces;
using MediatR;

namespace Application.UsesCases.Inputs
{
    /// <summary>
    /// Dto request
    /// </summary>
    /// <param name="NitEmisor"></param>
    /// <param name="TipoIdentificacion"></param>
    /// <param name="NumeroDocumento"></param>
    /// <param name="Metadatalist"></param>
    /// <param name="ApplicationType"></param>
    /// <param name="TipoDocumento"></param>
    public record RequestEnviarMetadataReceptorInputPort(
        string NitEmisor,
        string TipoIdentificacionEmisor,
        string NumeroDocumento,
        List<MetadataDto> Metadata,
        string TipoDocumento = "",
        int ApplicationType = 0
    ) : IRequest<IGeneralResponse>;
}
