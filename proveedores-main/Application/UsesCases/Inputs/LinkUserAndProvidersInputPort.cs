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
    /// Record Request de viculación de usuarios y proveedores.
    /// </summary>
    /// <param name="AplicationUser"></param>
    /// <param name="IdReceptor"></param>
    public record LinkUserAndProvidersInputPort(
        int AplicationUser,
        int IdReceptor,
        List<RequestProvidersAndStatusDto> Providers) : IRequest<IGeneralResponse>;
    
}
