using Azure;
using Azure.Core;
using Domain.Entity.Store_Procedure;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Mensajes;
using Domain.Request;
using Infrastructure.DataBase;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Infrastructure.Repository
{
    public class ProvidersRepository(ProvidersContext _context, IGeneralResponse _response) : IProvidersRepository
    {
        /// <summary>
        /// Metodo que permite realizar el proceso de activación de un proveedor desde el portal
        /// </summary>
        /// <param name="request"></param>
        /// <param name="logAzure"></param>
        /// <returns></returns>
        /// <exception cref="GeneralException"></exception>
        public async Task<IGeneralResponse> ActivateAsync(ActivateProviderRequest request, ILogAzure logAzure)
        {
            try
            {

                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, Mensajes.ProcesoActivacionLog_2, LevelMsn.Info);

                SqlParameter receptor = new("@id_receptor", request.IdReceptor);
                SqlParameter proveedor = new("@id_proveedor", request.IdProveedor);
                FormattableString query = FormattableStringFactory.Create("EXEC Portal.sp_activarProveedor {0}, {1}", receptor, proveedor);

                int result = await _context.Database.ExecuteSqlAsync(query);

                if (result == -1)
                {
                    _response.Codigo = 202;
                    _response.Mensaje = string.Format(Mensajes.ProcesoActivacionError_1, request.IdProveedor, request.IdReceptor);
                    _response.Resultado = "Error general!";
                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                }
                else
                {
                    _response.Codigo = 200;
                    _response.Mensaje = Mensajes.ProcesoActivacionCompletado;
                    _response.Resultado = "Procesado";
                    logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, _response.Mensaje, LevelMsn.Info);
                }

            }
            catch (Exception ex)
            {
                _response.Codigo = 500;
                _response.Mensaje = Mensajes.ProcesoActivacionError_1;
                _response.Resultado = "Error general!";
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"{_response.Mensaje}: Error: {JsonConvert.SerializeObject(ex)}", LevelMsn.Info);
            }
            return _response;
        }

        /// <summary>
        /// Metodo para obtener el paginado de proveedores asociados a un usuario en concreto.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="logAzure"></param>
        /// <returns></returns>
        /// <exception cref="GeneralException"></exception>
        public async Task<List<SpPaginateProviders>> PaginateProviders(PaginateProvidersRequest request, ILogAzure logAzure)
        {
            List<SpPaginateProviders> response = [];
            try
            {
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, string.Format(Mensajes.ProcesoPaginacionLog_2, request.AplicationUser), LevelMsn.Info);

                FormattableString query = FormattableStringFactory.Create("EXEC Portal.sp_list_provider_pag {0}, {1}, {2}, {3}, {4}, {5}, {6}",
                    new SqlParameter("@StartIndex", request.StartIndex),
                    new SqlParameter("@EndIndex", request.EndIndex),
                    new SqlParameter("@IdAplicationRoot", request.AplicationRoot),
                    new SqlParameter("@IdAplicationSubUser", request.AplicationUser),
                    new SqlParameter("@IdEnterprise", request.IdEnterprise),
                    new SqlParameter("@BuscarProvedor", request.BuscarProveedor ?? ""),
                    new SqlParameter("@DescOption", request.OrderByDesc));

                response = await _context.Set<SpPaginateProviders>().FromSql(query)
                    .AsNoTracking()
                    .ToListAsync();
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, string.Format(Mensajes.ProcesoPaginacionLog_3, response.Count), LevelMsn.Info);
            }
            catch (Exception ex)
            {
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Error: {JsonConvert.SerializeObject(ex)}", LevelMsn.Info);
                return response;
            }

            return response;
        }

        /// <summary>
        /// Metodo que permite activar y desactivar proveedores asignados a un sub usuario en el portal.
        /// </summary>
        /// <param name="proveedoresRequest"></param>
        /// <param name="logAzure"></param>
        /// <returns></returns>
        /// <exception cref="GeneralException"></exception>
        public async Task<IGeneralResponse> LinkUsersProvidersAsync(UsuariosProveedoresRequest proveedoresRequest, ILogAzure logAzure)
        {
            try
            {
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, Mensajes.ProcesoVincularUsuariosProveedores_log4, LevelMsn.Info);

                DataTable table = new();
                table.Columns.Add("AplicationUser", typeof(int));
                table.Columns.Add("IdProvider", typeof(int));
                table.Columns.Add("IdReceptor", typeof(int));
                table.Columns.Add("Estatus", typeof(bool));
                              

                foreach(var item in proveedoresRequest.ProvidersRequests)
                {
                    DataRow row = table.NewRow();

                    row["AplicationUser"] = proveedoresRequest.AplicationUser;
                    row["IdProvider"] = item.IdProvider;
                    row["IdReceptor"] = proveedoresRequest.IdReceptor;
                    row["Estatus"] = item.Estatus;

                    table.Rows.Add(row);
                }

                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, string.Format(Mensajes.ProcesoVincularUsuariosProveedores_log5, proveedoresRequest.ProvidersRequests.Count), LevelMsn.Info);

                SqlParameter sqlParameter = new()
                {
                    Value = table,
                    TypeName = "dbo.UserProviderTableType",
                    SqlDbType = SqlDbType.Structured,
                    ParameterName = "@UserProviderTable"
                };

                List<SpVincularUsuariosProveedores> resultado = await _context.Database
                    .SqlQueryRaw<SpVincularUsuariosProveedores>("EXEC Portal.assign_providers_to_users @UserProviderTable", sqlParameter)
                            .ToListAsync();


                if (resultado.Count > 0)
                {

                    SpVincularUsuariosProveedores? respuestaExitosa = resultado.FirstOrDefault(r => r.Codigo == 200);
                    SpVincularUsuariosProveedores? respuestaError = resultado.FirstOrDefault(r => r.Codigo == 404);

                    if (respuestaExitosa != null && respuestaError != null)
                    {
                       _response.Codigo = respuestaExitosa.Codigo;
                       _response.Mensaje = respuestaExitosa.Mensaje;
                       _response.Resultado = Mensajes.ProcesoVincularUsuariosProveedores_log7;
                       logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"{string.Format(Mensajes.ProcesoVincularUsuariosProveedores_log6, _response.Mensaje)} -- {_response.Resultado}", LevelMsn.Info);

                    }
                    else if (respuestaError != null && respuestaExitosa == null)
                    {
                        _response.Codigo = respuestaError.Codigo;
                        _response.Mensaje = $"{respuestaError.Mensaje} -- {Mensajes.ProcesoVincularUsuariosProveedores_log9}";
                        _response.Resultado = "Error";
                        logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"{_response.Mensaje}", LevelMsn.Error);

                    }else if (respuestaError == null && respuestaExitosa != null)
                    {
                        _response.Codigo = respuestaExitosa.Codigo;
                        _response.Mensaje = respuestaExitosa.Mensaje;
                        _response.Resultado = "Procesado";
                        logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"{string.Format(Mensajes.ProcesoVincularUsuariosProveedores_log6, _response.Mensaje)} -- {_response.Resultado}", LevelMsn.Info);
                    }
                }
                else
                {
                    _response.Codigo = 404;
                    _response.Mensaje = $"{Mensajes.ProcesoVincularUsuariosProveedores} -- {Mensajes.ProcesoVincularUsuariosProveedores_log8}";
                    _response.Resultado = "Error";
                }
            }
            catch(Exception ex)
            {
                logAzure.WriteComment(MethodBase.GetCurrentMethod()!.Name, string.Format(Mensajes.ProcesoVincularUsuariosProveedores_log2, JsonConvert.SerializeObject(ex)), LevelMsn.Info);
                _response.Codigo = 500;
                _response.Mensaje = string.Format(Mensajes.ProcesoVincularUsuariosProveedores_log2, ex.Message);
                throw new GeneralException(ex.Message, ex);
            }
            return _response;
        }
    }
}
