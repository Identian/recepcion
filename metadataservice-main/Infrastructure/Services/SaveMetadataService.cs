using Domain.Entity;
using Domain.Entity.Dto;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Mensajes;
using Domain.Request;
using Domain.TokenContext;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Infrastructure.Services
{
    /// <summary>
    /// Servicio de asociar data.metadata a un documento determinado
    /// </summary>
    /// <param name="_context"></param>
    /// <param name="_response"></param>
    /// <param name="_helper"></param>
    public class SaveMetadataService(MetadataContext _context, IGeneralResponse _response, IHelper _helper) : ISaveMetadataService
    {
        /// <summary>
        /// Metodo que permite asociar data.metadata a un documento determinado o su actualización.
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="data.metadata"></param>
        /// <param name="customJwt"></param>
        /// <param name="data.log"></param>
        /// <returns></returns>
        public async Task<IGeneralResponse> SendAsync(TransportData data)
        {
            try
            {
                
                data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de Eliminar Repetidos de los Metadatos", LevelMsn.Info);
                List<MetadataRequest> noduplicates = data.Metadata.Distinct().ToList();
                data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de Determinar codigo(s) con al menos dos valores", LevelMsn.Info);
                var agrupados = noduplicates.GroupBy(n => n.Code)
                    .Select(c => new { clave = c.Key, total = c.Count() });

                //Esta parte se cambio respecto a la original
                if (agrupados.Any(a => a.total > 1))
                {
                    _response.Codigo = 205;
                    _response.Mensaje = "Se está intentado incorporar al menos un código de data.metadata con al menos dos valores distintos";
                    _response.Resultado = "Procesado";
                    data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                    return _response;
                }

                data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Validación de metadatos", LevelMsn.Info);

                List<int> codigos = data.Metadata.Select(p => p.Code).ToList();

                data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de obtener la data.metadata disponible al enterprise que hace petición", LevelMsn.Info);

                var metadata_enterprise = (from cre in _context.CatReceptionExtensibles
                                           join ece in _context.EnterpriseCatReceptionExtensibles on cre.Id equals ece.IdCatReceptionExtensible
                                           join cdtrm in _context.CatDatatypeReceptionMetadata on cre.IdDatatypeReceptionMetadata equals cdtrm.Id
                                           where ece.IdEnterprise == data.IdEnterprise && ece.IsToSupplier == true && cre.IsActive && ece.Active
                                           select new
                                           {
                                               cre.Id,
                                               Code = cre.CodeExtensible,
                                               Idexttype = cre.IdExtensibleType,
                                               Typemetadata = cre.IdDatatypeReceptionMetadata,
                                               Typemetadacode = cdtrm.CodeMetadataDatatype,
                                               Typemetadadescr = cdtrm.Description,
                                               Islist = cre.IsListed,
                                               Ischild = cre.IsChild,
                                               Idroot = cre.IdRoot,
                                               Idm = ece.Id,
                                               Isrequired = ece.IsRequired
                                           }).ToList();

                var listables = data.Metadata
                                .Join(metadata_enterprise,
                                    mdti => mdti.Code,
                                    mtden => mtden.Code,
                                    (mdti, mtden) => new { mdti, mtden })
                                .Where(x => x.mtden.Islist)
                                .Select(x => new
                                {
                                    x.mtden.Code,
                                    x.mtden.Id,
                                    x.mdti.Value
                                })
                                .ToList();

                if (listables.Count > 0)
                {
                    var inlist = (from lt in listables
                              join crelv in _context.CatReceptionExtensibleListedValues on lt.Id equals crelv.IdCodeExtensible
                              where crelv.IsActive && (crelv.IdEnterprise == null || crelv.IdEnterprise == data.IdEnterprise) && (crelv.Value == lt.Value)
                              select new
                              {
                                  crelv.Id
                              }).ToList();

                    

                    if (inlist.Count == 0)
                    {
                        inlist = (from lt in listables
                                      join crelv in _context.CatReceptionExtensibleListedValues on lt.Id equals crelv.IdCodeExtensible
                                      where crelv.IsActive && (crelv.Value == lt.Value)
                                      select new
                                      {
                                          crelv.Id
                                      }).ToList();
                    }



                    if (listables.Count > inlist.Count)
                    {
                        _response.Codigo = 203;
                        _response.Mensaje = "Valor(es) suministrado(s) para Metadato(s) no corresponde(n) a la(s)  lista(s) definida(s)";
                        _response.Resultado = "Error";
                        data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                        return _response;
                    }
                }

                var ll = metadata_enterprise.Where(p => !codigos.Contains(p.Code)).ToList();

                int tipo1 = 0;
                int tipo3 = 0;
                foreach (var item in metadata_enterprise)
                {
                    if (item.Code == 1)
                    {
                        tipo1++;
                    }

                    if (item.Code == 3)
                    {
                        tipo3++;
                    }
                }
                if ((tipo1 > 1 || tipo3 > 1))
                {
                    _response.Codigo = 204;
                    _response.Mensaje = "Se intentó incorporar metadato(s) no definido(s) para el Cliente Receptor";
                    _response.Resultado = "Error";
                    data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                    return _response;
                }

                int temp_code;
                List<InvoiceReceptionExtensible> lincorporados = await _context.InvoiceReceptionExtensibles.Where(ire => ire.IdInvoiceReception == data.InvoiceId && ire.Active).ToListAsync();

                if (metadata_enterprise.Where(mt => mt.Isrequired).Any())
                {

                    if (lincorporados.Count > 0)
                    {
                        var new_req = data.Metadata
                                .Join(metadata_enterprise.Where(m => m.Isrequired),
                                      mdti => mdti.Code,
                                      mtden => mtden.Code,
                                      (mdti, mtden) => new
                                      {
                                          mtden.Code,
                                          mtden.Id,
                                          mdti.Value
                                      })
                                .Where(result =>
                                    !lincorporados.Any(inc => inc.IdEnterpriseCatReceptionExtensible == result.Id))
                                .ToList();

                        var old_req = lincorporados
                                .Join(metadata_enterprise.Where(m => m.Isrequired),
                                      linc => linc.IdEnterpriseCatReceptionExtensible,
                                      mtden => mtden.Idm,
                                      (linc, mtden) => new
                                      {
                                          mtden.Code,
                                          mtden.Id,
                                          linc.Value
                                      })
                                .ToList();

                        if (metadata_enterprise.Where(mt => mt.Isrequired).Count() > (new_req.Count + old_req.Count))
                        {
                            _response.Codigo = 206;
                            _response.Mensaje = "Existe(n) Metadato(s) requerido(s) que no fue(ron) suministrado(s)";
                            _response.Resultado = "Error";
                            data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                            return _response;
                        }

                    }
                    else
                    {

                        var requeridos = data.Metadata
                            .Join(metadata_enterprise.Where(m => m.Isrequired),
                                  mdti => mdti.Code,
                                  mtden => mtden.Code,
                                  (mdti, mtden) => new
                                  {
                                      mtden.Code,
                                      mtden.Id,
                                      mdti.Value
                                  })
                            .ToList();

                        if ((metadata_enterprise.Where(mt => mt.Isrequired).Count() > requeridos.Count))
                        {
                            _response.Codigo = 206;
                            _response.Mensaje = "Existe(n) Metadato(s) requerido(s) que no fue(ron) suministrado(s)";
                            _response.Resultado = "Error";
                            data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                            return _response;
                        }
                    }
                }

                int format_type;
                foreach (MetadataRequest meta in data.Metadata)
                {
                    try
                    {
                        InvoiceReceptionExtensible ltr = new();

                        var buscarMetaData = metadata_enterprise.Find(me => me.Code == meta.Code);

                        temp_code = buscarMetaData != null ? buscarMetaData.Idm : 0;
                        meta.Value = meta.Value.Trim();
                        format_type = buscarMetaData != null ? buscarMetaData.Typemetadacode : 0;

                        if (format_type != 1)
                        {
                            meta.Value = Regex.Replace(meta.Value, @"\s+", " ");
                        }
                        if (!_helper.ValidateFormatTypeMetadata(format_type, meta.Value))
                        {
                            string descript = buscarMetaData != null ? buscarMetaData.Typemetadadescr : "";
                            _response.Codigo = 207;
                            _response.Mensaje = String.Format("El valor {0} no tiene el formato definido para el tipo correspondiente para el  metadato código {1} ({2})", meta.Value, meta.Code, descript);
                            _response.Resultado = "Error";
                            data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                            return _response;
                        }
                        if (lincorporados.Count > 0)
                        {
                            try
                            {
                                InvoiceReceptionExtensible? rep = lincorporados.Where(li => li.IdEnterpriseCatReceptionExtensible == temp_code).FirstOrDefault();
                                if (rep is null)
                                {
                                    ltr.CreatedAt = DateTime.UtcNow;
                                    ltr.UpdatedAt = DateTime.UtcNow;
                                    ltr.IdInvoiceReception = data.InvoiceId;
                                    ltr.Internal1 = meta.Internal1;
                                    ltr.Internal2 = meta.Internal2;
                                    ltr.CreatedBy = data.InvoiceIdEnterpriseSupplier;
                                    ltr.UpdatedBy = data.InvoiceIdEnterpriseSupplier;
                                    ltr.IdEnterpriseCatReceptionExtensible = temp_code;
                                    ltr.Value = meta.Value;
                                    ltr.Active = true;

                                    await _context.InvoiceReceptionExtensibles.AddAsync(ltr);
                                }
                                else
                                {
                                    rep.UpdatedBy = data.InvoiceIdEnterpriseSupplier;
                                    rep.UpdatedAt = DateTime.UtcNow;
                                    rep.Value = meta.Value;
                                    _context.Entry(rep).State = EntityState.Modified;
                                }
                            }
                            catch (Exception)
                            {
                                ltr.CreatedAt = DateTime.UtcNow;
                                ltr.UpdatedAt = DateTime.UtcNow;
                                ltr.IdInvoiceReception = data.InvoiceId;
                                ltr.Internal1 = meta.Internal1;
                                ltr.Internal2 = meta.Internal2;
                                ltr.CreatedBy = data.InvoiceIdEnterpriseSupplier;
                                ltr.UpdatedBy = data.InvoiceIdEnterpriseSupplier;
                                ltr.IdEnterpriseCatReceptionExtensible = temp_code;
                                ltr.Value = meta.Value;
                                ltr.Active = true;

                                await _context.InvoiceReceptionExtensibles.AddAsync(ltr);
                            }
                        }
                        else
                        {
                            ltr.CreatedAt = DateTime.UtcNow;
                            ltr.UpdatedAt = DateTime.UtcNow;
                            ltr.IdInvoiceReception = data.InvoiceId;
                            ltr.Internal1 = meta.Internal1;
                            ltr.Internal2 = meta.Internal2;
                            ltr.CreatedBy = data.InvoiceIdEnterpriseSupplier;
                            ltr.UpdatedBy = data.InvoiceIdEnterpriseSupplier;
                            ltr.IdEnterpriseCatReceptionExtensible = temp_code;
                            ltr.Value = meta.Value;
                            ltr.Active = true;
                            await _context.InvoiceReceptionExtensibles.AddAsync(ltr);
                        }

                    }
                    catch (Exception mx)
                    {
                        _response.Codigo = 202;
                        _response.Mensaje = $"No se pudo incorporar data.metadata {mx.Message}";
                        _response.Resultado = "Error";
                        data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                        return _response;
                    }
                }
                int resultado = await _context.SaveChangesAsync();

                if (resultado > 0)
                {
                    _response.Codigo = 200;
                    _response.Mensaje = "Metadata enviada Satisfactoriamente";
                    _response.Resultado = "Procesado";
                    data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                }
            }
            catch (Exception ex)
            {
                _response.Codigo = 202;
                _response.Mensaje = $"No se pudo incorporar metadata {ex.Message}";
                _response.Resultado = "Error";
                data.LogAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                return _response;
            }
            return _response;
        }
    }
}
