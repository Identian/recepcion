using CapaDominio.Response;
using CasosDeUso.Inputs.CheckSilent;
using CasosDeUso.Inputs.GuardarTenant;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Radicacion.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Método que permite almacenar el Tenant id de Microsoft 365.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveTenantAuthorization")]
        public async Task<IRespuestaApi> SaveTenantAuthorization(SaveTenantInput dto)
        {
            return await _mediator.Send(dto);
        }
        /// <summary>
        /// Método que permite validar el estado del Tenant id de Microsoft 365.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CheckSilentAuthorization")]
        public async Task<IRespuestaApi> CheckSilentAuthorization(CheckSilentAuthorizationInput dto)
        {
            return await _mediator.Send(dto);
        }
    }
}
