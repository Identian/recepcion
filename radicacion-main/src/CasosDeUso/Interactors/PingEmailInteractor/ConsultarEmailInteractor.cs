using AutoMapper;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.RequestReceptor;
using CasosDeUso.Inputs.PingEmail;
using MediatR;

namespace CasosDeUso.Interactors.PingEmailInteractor
{
    public class ConsultarEmailInteractor : IRequestHandler<EmailPingInput, string>
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IMapper _mapper;
        public ConsultarEmailInteractor(IEmailRepository emailRepository, IMapper mapper)
        {
            _emailRepository = emailRepository;
            _mapper = mapper;
        }
        public async Task<string> Handle(EmailPingInput request, CancellationToken cancellationToken)
        {
            ICuentaCorreo cuentaCorreo = _mapper.Map<CuentaCorreo>(request);
            return await _emailRepository.ConsultarEmail(cuentaCorreo);
        }
    }
}
