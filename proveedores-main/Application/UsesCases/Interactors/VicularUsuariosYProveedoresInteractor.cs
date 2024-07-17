using Application.DTO;
using Application.UsesCases.Inputs;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Mensajes;
using Domain.Request;
using Mapster;
using MapsterMapper;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.UsesCases.Interactors
{
    public class VicularUsuariosYProveedoresInteractor(ILogAzure _logAzure, 
        IContextHelper _context,
        IMapper _mapper,
        IGeneralResponse _response,
        IProvidersRepository _providers) : IRequestHandler<LinkUserAndProvidersInputPort, IGeneralResponse>
    {
        public async Task<IGeneralResponse> Handle(LinkUserAndProvidersInputPort request, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            try
            {
                var jwt = _context.TokenContext();
                string url = _context.UrlContext();

                _logAzure.SetConfig(jwt, Mensajes.ProcesoVincularUsuariosProveedores, ApplicationType.Portal, url);

                UsuariosProveedoresRequest requestDomain = _mapper.Map<UsuariosProveedoresRequest>(request);
                List<ProvidersRequest> providers = [];

                foreach (var p in request.Providers)
                {
                    ProvidersRequest pr = p.Adapt<ProvidersRequest>();
                    providers.Add(pr);
                }
                requestDomain.ProvidersRequests = providers;

                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, Mensajes.ProcesoVincularUsuariosProveedores_log1, LevelMsn.Info);

                _response = await _providers.LinkUsersProvidersAsync(requestDomain, _logAzure);

                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, string.Format(Mensajes.ProcesoVincularUsuariosProveedores_log3, _response.Codigo), LevelMsn.Info);

            }
            catch (Exception ex)
            {
                _logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, string.Format(Mensajes.ProcesoVincularUsuariosProveedores_log2, JsonConvert.SerializeObject(ex)), LevelMsn.Info);
                _response.Codigo = 500;
                _response.Mensaje = $"{Mensajes.ProcesoVincularUsuariosProveedores} -- {string.Format(Mensajes.ProcesoVincularUsuariosProveedores_log2, ex.Message)}";
                throw new GeneralException(ex.Message, ex);
            }
            finally
            {
                stopwatch.Stop();
                LevelMsn level = (_response.Codigo != 200) ? LevelMsn.Error : LevelMsn.Info;
                _logAzure.Savelog($"{_response.Codigo}", $"{Mensajes.ProcesoVincularUsuariosProveedores} -- {_response.Mensaje}", ref stopwatch, level);
            }

            return _response;
        }
    }
}
