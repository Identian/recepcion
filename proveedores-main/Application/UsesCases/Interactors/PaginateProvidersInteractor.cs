using Application.DTO;
using Application.UsesCases.Inputs;
using Domain.Entity.Store_Procedure;
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
    /// <summary>
    /// Clase encargada de interactuar con el repositorio, obtener la respuesta de base de datos mapear los datos y responder.
    /// </summary>
    /// <param name="_logAzure"></param>
    /// <param name="_providers"></param>
    /// <param name="_helper"></param>
    /// <param name="_response"></param>
    public class PaginateProvidersInteractor(
        ILogAzure _logAzure,
        IProvidersRepository _providers,
        IContextHelper _helper,
        IGeneralResponse _response) : IRequestHandler<PaginateProvidersInputPort, PaginateResponse>
    {
        public async Task<PaginateResponse> Handle(PaginateProvidersInputPort request, CancellationToken cancellationToken)
        {
            Stopwatch time = new();
            time.Start();

            PaginateResponse response = new();
            try
            {
                CustomJwtTokenContext context = _helper.TokenContext();
                string url = _helper.UrlContext();
                _logAzure.SetConfig(context, Mensajes.ProcesoPaginacion, ApplicationType.Portal, url);

                PaginateProvidersRequest requestDomain = request.Adapt<PaginateProvidersRequest>();

                requestDomain.IdEnterprise = int.Parse(context.EnterpiseId);

                List<SpPaginateProviders> lista = await _providers.PaginateProviders(requestDomain, _logAzure);

                if (lista.Count > 0 && lista[0].IdProvider != null && lista[0].IdProvider != 0)
                {
                    List<PaginateDto> listaDto = [];
                    foreach(var i in lista)
                    {
                        listaDto.Add(i.Adapt<PaginateDto>());
                    }

                    response = new PaginateResponse
                    {
                        TotalRegistros = lista.First().TotalRegistros,
                        TotalFiltrado = lista.First().TotalFiltrados,
                        Providers = listaDto
                    };

                    _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, string.Format(Mensajes.ProcesoPaginacionLog_1, response.Providers.Count), LevelMsn.Info);
                    _response.Codigo = 200;
                    _response.Mensaje = $"{Mensajes.ProcesoPaginacion} -- {string.Format(Mensajes.ProcesoPaginacionCompletado, response.Providers.Count, response.TotalRegistros)}";
                }
                else
                {
                    response = new PaginateResponse
                    {
                        TotalFiltrado = 0,
                        TotalRegistros = 0,
                        Providers = []
                    };

                    _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"{Mensajes.ProcesoPaginaciónError_2} {response.Providers.Count}", LevelMsn.Info);
                    _response.Codigo = 500;
                    _response.Mensaje = $"{Mensajes.ProcesoPaginacion} -- {Mensajes.ProcesoPaginaciónError_2}";
                }
            }catch (Exception ex)
            {
                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, string.Format(Mensajes.ProcesoPaginaciónError_3, JsonConvert.SerializeObject(ex)), LevelMsn.Error);
                _response.Codigo= 500;
                _response.Mensaje = $"{Mensajes.ProcesoPaginacion} -- {string.Format(Mensajes.ProcesoPaginaciónError_3, "error general")}";
                _response.Resultado = "Error";
            }
            finally
            {
                time.Stop();
                LevelMsn level = (_response.Codigo != 200) ? LevelMsn.Error : LevelMsn.Info;
                _logAzure.Savelog($"{_response.Codigo}", _response.Mensaje, ref time, level);
            }
            return response;
        }
    }
}
