using CapaDominio.Auth;
using CapaDominio.Errors;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Response;
using CasosDeUso.Inputs.CheckSilent;
using MediatR;

namespace CasosDeUso.Interactors.CheckSilentInteractor
{
    public class CheckSilentAuthorizationInterator : IRequestHandler<CheckSilentAuthorizationInput, IRespuestaApi>
    {
        private readonly ICheckSilentAuthorizationRepository _checkSilent;

        public CheckSilentAuthorizationInterator(ICheckSilentAuthorizationRepository checkSilent)
        {
            _checkSilent = checkSilent;
        }
        public async Task<IRespuestaApi> Handle(CheckSilentAuthorizationInput request, CancellationToken cancellationToken)
        {
            IRespuestaApi respuestaApi = new RespuestaApi();
            if (!String.IsNullOrEmpty(request.State))
            {
                CheckSilentAuthorization checkSilent = new()
                {
                    State = request.State,
                };
                respuestaApi = await _checkSilent.CheckSilent(checkSilent);
            }
            else
            {
                respuestaApi.Codigo = 400;
                respuestaApi.Mensaje = ErrorsCodes._400;
                return respuestaApi;
            }
            return respuestaApi;
        }
    }
}
