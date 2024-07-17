using Application.DTO;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UsesCases.Inputs
{
    /// <summary>
    /// DTO Record para asociar metadata desde el emisor
    /// Usando TokenUser y TokenPassword del emisor de la factura
    /// </summary>
    /// <param name="NumeroDocumento"></param>
    /// <param name="Metadata"></param>
    /// <param name="TipoDocumento"></param>
    public record RequestEnviarMetadataEmisorInputPort(
        string NumeroDocumento,
        List<MetadataDto> Metadata,
        string TipoDocumento = "01"
    ) : IRequest<IGeneralResponse>;
}
