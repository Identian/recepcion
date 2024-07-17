using Application.UsesCases.Inputs;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Mensajes;
using Domain.Request;
using Domain.TokenContext;
using Mapster;
using MapsterMapper;
using MediatR;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace Application.UsesCases.Interactors
{
    public class EnviarMetadataReceptorInteractor(
            ILogAzure _logAzure, 
            IContextHelper _contextHelper,
            IGeneralResponse _response,
            IMapper _mapper,
            IEnviarMetadataReceptorRepository _repository) : IRequestHandler<RequestEnviarMetadataReceptorInputPort, IGeneralResponse>
    {
        public async Task<IGeneralResponse> Handle(RequestEnviarMetadataReceptorInputPort request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            try
            {
                CustomJwtTokenContext contex = _contextHelper.TokenContext();

                string url = _contextHelper.UrlContext();
                ApplicationType app = request.ApplicationType == 0 ? ApplicationType.Integracion : ApplicationType.Portal;
                
                _logAzure.SetConfig(contex, Mensajes.ProccessMetadataController, app, url);
                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de empezar con el registro de metadata", LevelMsn.Info, stopwatch.ElapsedMilliseconds);

                RequestEnviarMetadata requestmetadata = _mapper.Map<RequestEnviarMetadata>(request);
                List<MetadataRequest> metadata = request.Metadata.Select(x => x.Adapt<MetadataRequest>()).ToList();
                requestmetadata.Metadata = metadata;

                _response = await _repository.SaveMetadata(requestmetadata, contex, _logAzure);

                if (_response.Codigo != 200)
                {
                    _response.Mensaje = string.Format(Mensajes.ProcesoMetadataError_2, request.NumeroDocumento, request.NitEmisor, contex.EnterpriseNit, _response.Mensaje);
                }
            }
            catch(Exception ex)
            {
                _response.Codigo = 500;
                _response.Mensaje = $"Error: en el servicio {ex.Message}";
                _response.Resultado = $"Error.";
                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Error: {JsonConvert.SerializeObject(ex)}", LevelMsn.Info, stopwatch.ElapsedMilliseconds);
                throw new GeneralException(ex.Message, ex);
            }
            finally
            {
                stopwatch.Stop();
                LevelMsn level = (_response.Codigo != 200) ? LevelMsn.Error : LevelMsn.Info;
                _logAzure.Savelog($"{_response.Codigo}", $"{Mensajes.ProcesoMetadata}: {_response.Mensaje}", ref stopwatch, level, request.NumeroDocumento);
            }
            return _response;
        }
    }
}
