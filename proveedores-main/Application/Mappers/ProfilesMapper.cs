using Application.UsesCases.Inputs;
using Domain.Request;
using Mapster;

namespace Application.Mappers
{
    public class ProfilesMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {

            config.NewConfig<LinkUserAndProvidersInputPort, UsuariosProveedoresRequest>()
                .Map(src => src.AplicationUser, des => des.AplicationUser)
                .Map(src => src.IdReceptor, des => des.IdReceptor);
            
        }
    }
}
