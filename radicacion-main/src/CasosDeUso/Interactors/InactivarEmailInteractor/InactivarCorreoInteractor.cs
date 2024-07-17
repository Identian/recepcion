using AutoMapper;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Receptor;
using CasosDeUso.Inputs.InactivarCorreo;
using MediatR;

namespace CasosDeUso.Interactors.InactivarEmailInteractor
{
    public class InactivarCorreoInteractor : IRequestHandler<InactivarEmailInput, string>
    {
        private readonly IEmailInactivarRepository _emailInactivarRepository;
        private readonly IMapper _mapper;

        public InactivarCorreoInteractor(IEmailInactivarRepository emailInactivarRepository, IMapper mapper)
        {
            _emailInactivarRepository = emailInactivarRepository;
            _mapper = mapper;
        }
        public async Task<string> Handle(InactivarEmailInput request, CancellationToken cancellationToken)
        {
            ConsultaEmail inactivarCorreo = _mapper.Map<ConsultaEmail>(request);
            return await _emailInactivarRepository.InactivarEmail(inactivarCorreo);
        }
    }
}
