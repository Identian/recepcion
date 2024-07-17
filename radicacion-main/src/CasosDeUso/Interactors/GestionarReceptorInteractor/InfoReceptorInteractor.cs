using AutoMapper;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.RequestReceptor;
using CapaDominio.Response;
using CasosDeUso.Exceptions;
using CasosDeUso.Inputs.GestionarReceptor;
using MediatR;

namespace CasosDeUso.Interactors.GestionarReceptorInteractor
{
    public class InfoReceptorInteractor : IRequestHandler<InfoReceptorInput, IRespuesta>
    {
        readonly IGestionReceptoresRepository _repository;
        private readonly IMapper _mapper;
        public InfoReceptorInteractor(IGestionReceptoresRepository gestionReceptoresRepository, IMapper mapper)
        {
            _repository = gestionReceptoresRepository;
            _mapper = mapper;
        }
        public async Task<IRespuesta> Handle(InfoReceptorInput request, CancellationToken cancellationToken)
        {
            IRespuesta response;
            try
            {
                InfoReceptor info = new()
                {
                    r = _mapper.Map<Receptor>(request.r),
                    cb = _mapper.Map<CuentaCorreo>(request.cb)
                };

                response = await _repository.GestionarReceptor(info);
            }
            catch (Exception ex)
            {
                throw new GeneralException(ex.Message, ex);
            }
            return response;
        }
    }
}

