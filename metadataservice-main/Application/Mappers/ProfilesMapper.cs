using Application.UsesCases.Inputs;
using Domain.Request;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class ProfilesMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            //Mapeo del record inputport a clase de dominio RequestEnviarMetadata
            config.NewConfig<RequestEnviarMetadataReceptorInputPort, RequestEnviarMetadata>()
                .Map(des => des.NitEmisor, src => src.NitEmisor)
                .Map(des => des.TipoIdentificacionEmisor, src => src.TipoIdentificacionEmisor)
                .Map(des => des.NumeroDocumento, src => src.NumeroDocumento)
                .Map(des => des.TipoDocumento, src => src.TipoDocumento);

            //Mapeo del record inputport a clase de dominio RequestEnviarMetadata
            config.NewConfig<RequestEnviarMetadataEmisorInputPort, RequestEnviarMetadata>()
                .Map(des => des.NumeroDocumento, src => src.NumeroDocumento)
                .Map(des => des.TipoDocumento, src => src.TipoDocumento);
        }
    }
}
