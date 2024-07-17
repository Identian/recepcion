using Domain.Entity;
using Domain.Entity.Dto;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Mensajes;
using Domain.Request;
using Domain.TokenContext;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;
using System.Text.RegularExpressions;


namespace Infrastructure.Repository
{
    public class EnviarMetadataReceptorRepository(
        MetadataContext _context, 
        IGeneralResponse _response,
        ISaveMetadataService _metadataService) : IEnviarMetadataReceptorRepository
    {
        public async Task<IGeneralResponse> SaveMetadata(RequestEnviarMetadata request, CustomJwtTokenContext customJwt, ILogAzure logAzure)
        {
            try
            {
                request.TipoDocumento = string.IsNullOrEmpty(request.TipoDocumento) ? "01" : request.TipoDocumento;
                
                int enterpriseId = 0;
                if (int.TryParse(customJwt.EnterpiseId, out int id)) {
                    enterpriseId = id;
                }

                InvoiceReception? invoiceReception = await _context.InvoiceReceptions
                    .Where(i => i.DocumentId == request.NumeroDocumento && i.IdEnterpriseSupplier == enterpriseId
                    && i.PartyIdentificationId == request.NitEmisor
                    && i.DocumentType == request.TipoDocumento
                    && i.SchemeId == request.TipoIdentificacionEmisor && i.Active == 1).FirstOrDefaultAsync();


                if (invoiceReception is null)
                {
                    _response.Codigo = 105;
                    _response.Mensaje = string.Format(Mensajes.ProcesoMetadataLog_2, request.NumeroDocumento, request.NitEmisor, customJwt.EnterpriseNit);
                    _response.Resultado = "Procesado";
                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, string.Format(Mensajes.ProcesoMetadataLog_2, request.NumeroDocumento, request.NitEmisor, customJwt.EnterpriseNit), LevelMsn.Info);
                    return _response;
                }

               
                TransportData data = new() { 
                    IdEnterprise = enterpriseId,
                    InvoiceId = invoiceReception.Id,
                    LogAzure = logAzure,
                    InvoiceIdEnterpriseSupplier = invoiceReception.IdEnterpriseSupplier,
                    Metadata = request.Metadata
                };


                _response = await _metadataService.SendAsync(data);
            }
            catch (Exception ex)
            {
                _response.Codigo = 202;
                _response.Mensaje = string.Format(Mensajes.ProcesoMetadataError_1, ex.Message);
                _response.Resultado = "Procesado";
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Error: {JsonConvert.SerializeObject(ex)}", LevelMsn.Info);
                return _response;
            }
            return _response;
        }
    }
}
