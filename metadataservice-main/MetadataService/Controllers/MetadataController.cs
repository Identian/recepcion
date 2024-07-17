using Application.UsesCases.Inputs;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MetadataService.Web.Controllers
{
    /// <summary>
    /// Controlador de metadata
    /// </summary>
    /// <param name="_mediator"></param>
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MetadataController(IMediator _mediator) : Controller
    {

        /// <summary>
        /// Metodo de envio de metadata por parte del receptor.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/api/integracion/metadata/enviar")]
        public async Task<IActionResult> PostMetadataEnviar([FromBody] RequestEnviarMetadataReceptorInputPort request)
        {
           IGeneralResponse response = await _mediator.Send(request);
           return new ObjectResult(response);
        }

        /// <summary>
        /// Metodo de asociar metadata por parte delemisor de la factura.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("/api/integracion/metadata/enviarEmisor")]
        public async Task<IActionResult> PostMetaDataEnviarEmisor([FromBody] RequestEnviarMetadataEmisorInputPort request)
        {
            IGeneralResponse response = await _mediator.Send(request);
            return new ObjectResult(response);
        }
    }
}
