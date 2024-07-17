using Application.UsesCases.Inputs;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Mensajes;
using Domain.Request;
using Domain.TokenContext;
using Mapster;
using MediatR;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;

namespace Application.UsesCases.Interactors
{
    public class ActivateProvidersInteractor(
        ILogAzure _logAzure,
        IContextHelper _helper,
        IGeneralResponse _response,
        IProvidersRepository _providers) : IRequestHandler<ActivateProviderRequestInputPort, IGeneralResponse>
    {
        public async Task<IGeneralResponse> Handle(ActivateProviderRequestInputPort request, CancellationToken cancellationToken)
        {
            Stopwatch time = new();
            time.Start();
            try
            {
                CustomJwtTokenContext context = _helper.TokenContext();
                string url = _helper.UrlContext();
                _logAzure.SetConfig(context, Mensajes.ProcesoActivacionProveedores, ApplicationType.Portal, url);

                ActivateProviderRequest providerRequest = request.Adapt<ActivateProviderRequest>();

                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, Mensajes.ProcesoActivacionProveedoresLog_1, LevelMsn.Info, 0);

                _response = await _providers.ActivateAsync(providerRequest, _logAzure);
            }
            catch (Exception ex)
            {
                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"{_response.Mensaje} - {JsonConvert.SerializeObject(ex)}", LevelMsn.Info, 0);
            }
            finally
            {
                time.Stop();
                LevelMsn level = (_response.Codigo != 200) ? LevelMsn.Error : LevelMsn.Info;
                _logAzure.Savelog($"{_response.Codigo}", $"{Mensajes.ProcesoActivacionProveedores}: {_response.Mensaje}", ref time, level);
            }
            return _response;
        }
    }
}
