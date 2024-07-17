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
using System.Globalization;
using System.Reflection;


namespace Application.UsesCases.Interactors
{
    public class EnviarMetadataEmisorInteractor(
        ILogAzure _logAzure,
        IContextHelper _contextHelper,
        IMapper _mapper,
        IGeneralResponse _response,
        IEnviarMetadataEmisorRepository _enviarMetadata
        ) : IRequestHandler<RequestEnviarMetadataEmisorInputPort, IGeneralResponse>
    {
        public async Task<IGeneralResponse> Handle(RequestEnviarMetadataEmisorInputPort request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            try
            {
                CustomJwtTokenContext contex = _contextHelper.TokenContext();
                string url = _contextHelper.UrlContext();

                _logAzure.SetConfig(contex, Mensajes.ProccessMetadataController, ApplicationType.Integracion, url);
                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, Mensajes.ProcesoMetadataLog_1, LevelMsn.Info);

                RequestEnviarMetadata requestmetadata = _mapper.Map<RequestEnviarMetadata>(request);
                List<MetadataRequest> metadata = request.Metadata.Select(x => x.Adapt<MetadataRequest>()).ToList();
                requestmetadata.Metadata = metadata;

                _response = await _enviarMetadata.SendMetadataEmisor(requestmetadata, contex, _logAzure);
            }
            catch (Exception ex)
            {
                _response.Codigo = 102;
                _response.Mensaje = $"Se produjo un error en el servicio.";
                _response.Resultado = $"Error.";
                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Error: {JsonConvert.SerializeObject(ex)}", LevelMsn.Error);
                throw new GeneralException(ex.Message, ex);
            }
            finally
            {
                stopwatch.Stop();
                LevelMsn level = (_response.Codigo != 200) ? LevelMsn.Error : LevelMsn.Info;
                _logAzure.Savelog($"{_response.Codigo}", $"{Mensajes.ProcesoMetadata} - {_response.Mensaje}", ref stopwatch, level, request.NumeroDocumento);
            }
            return _response;
        }
    }
}
