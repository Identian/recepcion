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
    /// Request para la activación de proveedores
    /// </summary>
    /// <param name="IdReceptor"></param>
    /// <param name="IdProveedor"></param>
    public record ActivateProviderRequestInputPort(int IdReceptor, int IdProveedor) : IRequest<IGeneralResponse>;
}
