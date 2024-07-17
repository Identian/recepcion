using Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UsesCases.Inputs
{
    /// <summary>
    /// Dto request para la solicitud de paginacion de proveedores asignados o no a un usuario en concreto
    /// </summary>
    /// <param name="StartIndex">Registro de inicio</param>
    /// <param name="EndIndex">Registro limite</param>
    /// <param name="AplicationRoot">Identificador del usuario con permisos de asignar usuarios en el portal</param>
    /// <param name="AplicationUser">Identificador del usuario al que se le estan asignando los proveedores</param>
    /// <param name="IdEnterprise">Identificador de la empresa</param>
    /// <param name="BuscarProveedor">Si se rerwuiere buscar algun proveedor por nombre o nit</param>
    /// <param name="OrderByDesc">Forma de organizar los datos</param>
    public record PaginateProvidersInputPort( 
        int? StartIndex,
        int? EndIndex,
        int? AplicationRoot,
        int? AplicationUser,
        int? IdEnterprise,
        string BuscarProveedor,
        bool? OrderByDesc = false): IRequest<PaginateResponse>;
}
