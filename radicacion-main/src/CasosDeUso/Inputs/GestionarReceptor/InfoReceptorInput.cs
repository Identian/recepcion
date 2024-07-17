using CapaDominio.Response;
using DTO.GestionCorreoDto;
using MediatR;

namespace CasosDeUso.Inputs.GestionarReceptor
{
    public class InfoReceptorInput : InfoReceptorDto, IRequest<IRespuesta>
    {
    }
}
