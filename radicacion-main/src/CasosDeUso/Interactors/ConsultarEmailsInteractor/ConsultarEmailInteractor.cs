using AutoMapper;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Receptor;
using CasosDeUso.Inputs.ConsultarEmail;
using MediatR;

namespace CasosDeUso.Interactors.ConsultarEmailsInteractor
{
    public class ConsultarEmailInteractor : IRequestHandler<ConsultarEmailInput, string>
    {
        private readonly IConsultarEmailRepository _emailRepository;
        private readonly IMapper _mapper;
        public ConsultarEmailInteractor(IConsultarEmailRepository emailRepository, IMapper mapper)
        {
            _emailRepository = emailRepository;
            _mapper = mapper;
        }
        public async Task<string> Handle(ConsultarEmailInput request, CancellationToken cancellationToken)
        {
            ConsultaEmail datosConsulta = _mapper.Map<ConsultaEmail>(request);
            string resultado = await _emailRepository.Consultar(datosConsulta);
            return resultado;
        }
    }
}
