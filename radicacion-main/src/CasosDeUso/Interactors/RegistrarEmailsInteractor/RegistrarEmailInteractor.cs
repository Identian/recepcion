using AutoMapper;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.RequestReceptor;
using CasosDeUso.Inputs.RegistrarEmail;
using MediatR;

namespace CasosDeUso.Interactors.RegistrarEmailsInteractor
{
    /// <summary>
    /// Clase intermediaria en el uso de mediator, Guardar o actualizar un nueva cuenta de correo
    /// </summary>
    internal class RegistrarEmailInteractor : IRequestHandler<RegistrarEmailInput, string>
    {
        private readonly IRegistrarEmailRepository _registrarEmail;
        private readonly IMapper _mapper;

        public RegistrarEmailInteractor(IRegistrarEmailRepository registrarEmail, IMapper mapper)
        {
            _registrarEmail = registrarEmail;
            _mapper = mapper;
        }
        public async Task<string> Handle(RegistrarEmailInput request, CancellationToken cancellationToken)
        {
            CuentaCorreoGuardar cuentaCorreo = _mapper.Map<CuentaCorreoGuardar>(request);
            return await _registrarEmail.Registrar(cuentaCorreo);
        }
    }
}
