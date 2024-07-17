using CapaDominio.Auth;
using CapaDominio.Errors;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Response;
using CasosDeUso.Inputs.GuardarTenant;
using MediatR;

namespace CasosDeUso.Interactors.GuardarTenantInteractor
{
    internal class SaveTenantInteractor : IRequestHandler<SaveTenantInput, IRespuestaApi>
    {
        private readonly IRespuestaApi _respuestaApi;
        private readonly ISaveTenantRepository _saveTenantRepository;

        public SaveTenantInteractor(IRespuestaApi respuestaApi, ISaveTenantRepository saveTenantRepository)
        {
            _respuestaApi = respuestaApi;
            _saveTenantRepository = saveTenantRepository;
        }
        public async Task<IRespuestaApi> Handle(SaveTenantInput request, CancellationToken cancellationToken)
        {
            IRespuestaApi resultado;
            if (request.State!.Equals(null) || request.TenantId!.Equals(null))
            {
                _respuestaApi.Codigo = 400;
                _respuestaApi.Mensaje = ErrorsCodes._400;
                return _respuestaApi;
            }
            Authenticate authenticate = new()
            {
                State = request.State,
                TenantId = request.TenantId,
            };
            resultado = await _saveTenantRepository.SaveTenant(authenticate);
            return resultado;
        }
    }
}
