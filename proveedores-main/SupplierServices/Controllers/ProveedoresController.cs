using Application.DTO;
using Application.UsesCases.Inputs;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SupplierServices.Web.Controllers
{
    /// <summary>
    /// Controlador de proveedores
    /// </summary>
    /// <param name="_mediator"></param>
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ProveedoresController(IMediator _mediator) : Controller
    {
        /// <summary>
        /// Metodo de activación de proveedores
        /// </summary>
        /// <param name="resquest"></param>
        /// <returns></returns>
        [HttpPost("/api/proveedores/activar")]
        public async Task<IActionResult> ActivateProviders([FromBody] ActivateProviderRequestInputPort resquest)
        {
            IGeneralResponse response = await _mediator.Send(resquest);
            return new ObjectResult(response);
        }

        /// <summary>
        /// Obtener los proveedores asociados a un usuario
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/api/proveedores/listProviders")]
        public async Task<IActionResult> ListProviders([FromBody] PaginateProvidersInputPort request)
        {
            PaginateResponse response = await _mediator.Send(request);
            return new ObjectResult(response);
        }

        /// <summary>
        /// Metodo de vincular usuarios y proveedores.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/api/proveedores/vincularUsuarioProveedores")]
        public async Task<IActionResult> LinkUsersAndProviders([FromBody] LinkUserAndProvidersInputPort request)
        {
            IGeneralResponse response = await _mediator.Send(request);
            return new ObjectResult(response);
        }
    }
}
