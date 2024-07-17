using Domain.Entity;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Request;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Domain.TokenContext;
using Infrastructure.DataBase;
using Domain.Mensajes;
using Newtonsoft.Json;
using Domain.Entity.Dto;
namespace Infrastructure.Repository
{
    public class EnviarMetadataEmisorRepository(MetadataContext _context, IGeneralResponse _response, ISaveMetadataService _metadataService) : IEnviarMetadataEmisorRepository
    {
        public async Task<IGeneralResponse> SendMetadataEmisor(RequestEnviarMetadata request, CustomJwtTokenContext customJwt, ILogAzure logAzure)
        {
            try
            {
                request.TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento;

                int enterpriseId = 0;
                if (int.TryParse(customJwt.EnterpiseId, out int id))
                {
                    enterpriseId = id;
                }

                InvoiceReception? invoiceReception = await _context.InvoiceReceptions
                    .Where(i => i.DocumentId == request.NumeroDocumento 
                        && i.IdEnterpriseIssues == enterpriseId
                        && i.PartyIdentificationId == customJwt.EnterpriseNit
                        && i.DocumentType == request.TipoDocumento
                        && i.SchemeId == customJwt.EnterpiseSchemeId 
                        && i.Active == 1).FirstOrDefaultAsync();

                if (invoiceReception is null)
                {
                    _response.Codigo = 202;
                    _response.Mensaje = string.Format(Mensajes.ProcesoMetadataLog_3, request.NumeroDocumento, customJwt.EnterpriseNit);
                    _response.Resultado = "Error";
                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                    return _response;
                }

                TransportData data = new() { 
                    IdEnterprise = invoiceReception.IdEnterpriseSupplier,
                    InvoiceId = invoiceReception.Id,
                    InvoiceIdEnterpriseSupplier = enterpriseId,
                    LogAzure = logAzure,
                    Metadata = request.Metadata
                };
                
                _response = await _metadataService.SendAsync(data);
            }
            catch (Exception ex)
            {
                _response.Codigo = 202;
                _response.Mensaje = string.Format(Mensajes.ProcesoMetadataError_1, ex.Message);
                _response.Resultado = "Error";
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Error: {JsonConvert.SerializeObject(ex)}", LevelMsn.Info);
                return _response;
            }
            return _response;
        }


    }
}
