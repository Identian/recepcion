using CapaDominio.Entity;
using CapaDominio.Enums.Logs;
using CapaDominio.Enums.TipoAutenticacion;
using CapaDominio.Interfaces.IDB;
using CapaDominio.Interfaces.IHelpers;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Invoice;
using CapaDominio.RequestReceptor;
using CapaDominio.Response;
using Infraestructura.Logs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Infraestructura.DB
{
    public class DataBase : IDataBase
    {
        private
        readonly IConfiguration _configuration;
        readonly IRespuesta _respuesta;
        readonly IRespuestaApiConsultar _respuestaApiConsultar;
        readonly IListadoCorreo _listadoCorreo;
        readonly IObjectConversion<CuentaCorreo> _objectConversion;

        private SqlConnection? _conexion;
        public DataBase(IConfiguration configuration, IRespuesta respuesta, IRespuestaApiConsultar respuestaApiConsultar, IListadoCorreo listadoCorreo, IObjectConversion<CuentaCorreo> objectConversion)
        {
            _configuration = configuration;
            _respuesta = respuesta;
            _respuestaApiConsultar = respuestaApiConsultar;
            _listadoCorreo = listadoCorreo;
            _objectConversion = objectConversion;
        }

        public async Task Conexion()
        {
            try
            {
                string connectionString = _configuration.GetSection("ConnectionString").Value ?? "";
                _conexion = new SqlConnection(connectionString);
                if (_conexion.State != ConnectionState.Open)
                { 
                    await _conexion.OpenAsync(); 
                }
            }
            catch (Exception)
            {
                Environment.Exit(-1);
            }
        }

        async Task CloseConexion()
        {
            if (_conexion.State == ConnectionState.Open)
            {
                await _conexion.CloseAsync();
            }
        }

        /// <summary>
        /// Registrar configuración de correo electronico en la base de datos
        /// </summary>
        /// <param name="parametros"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<IRespuesta> RegistrarCuentaCorreoReceptor(CuentaCorreoGuardar parametros, ILogAzure log)
        {
            IRespuesta result = _respuesta;
            result.SetMetodo($"{nameof(DataBase)} . {nameof(RegistrarCuentaCorreoReceptor)}");

            if (parametros.TipoAutenticacion == TipoAutenticacion.AUTENTICACION_BASICA)
            {
                parametros.Clave = IUtils.Encriptar(parametros.Clave!);
            }

            try
            {
                await this.Conexion();
                //Registro la Cuenta Correo
                using SqlCommand cmd = new("sp_AGE_GuardarEmailReceptor", _conexion);
                DateTime minimumSQLDateTime = new(1753, 1, 1);

                var resul = DateTime.TryParse(parametros.Expires_in, out DateTime auxTokeExpiration);

                if (auxTokeExpiration < minimumSQLDateTime)
                { auxTokeExpiration = DateTime.UtcNow; }

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@NIT", parametros.Nit);
                cmd.Parameters.AddWithValue("@TipoIdentificadorReceptor", parametros.TipoIdentificadorReceptor);
                cmd.Parameters.AddWithValue("@Servidor", parametros.Servidor);
                cmd.Parameters.AddWithValue("@Puerto", parametros.Puerto);
                cmd.Parameters.AddWithValue("@UsarSSL", parametros.UsarSSL);
                cmd.Parameters.AddWithValue("@Usuario", parametros.Usuario);
                cmd.Parameters.AddWithValue("@Clave", parametros.Clave);
                cmd.Parameters.AddWithValue("@AccessToken", parametros.AccessToken ?? "");
                cmd.Parameters.AddWithValue("@RefreshToken", parametros.RefreshToken ?? "");
                cmd.Parameters.AddWithValue("@AccessTokenExpirationUTC", auxTokeExpiration);
                cmd.Parameters.AddWithValue("@Entrada", parametros.BandejaEntrada);
                cmd.Parameters.AddWithValue("@Descargados", parametros.BandejaDescargados);
                cmd.Parameters.AddWithValue("@Erroneos", parametros.BandejaErroneos);
                cmd.Parameters.AddWithValue("@Otros", parametros.BandejaOtros);
                cmd.Parameters.AddWithValue("@TipoAutenticacion", parametros.TipoAutenticacion.ToString());
                cmd.Parameters.Add("@CodSalida", SqlDbType.Int);
                cmd.Parameters["@CodSalida"].Direction = ParameterDirection.Output;
                await cmd.ExecuteNonQueryAsync();

                int _CodigoSalida = (int)cmd.Parameters["@CodSalida"].Value;

                if (_CodigoSalida == 1)
                {
                    result.Codigo = 999;
                    result.Resultado = false;
                    result.Descripcion = "El Número de identificación " + parametros.Nit + " y Tipo de identificación " + parametros.TipoIdentificadorReceptor + " ingresado no existe.";
                    return result;
                }

                if (_CodigoSalida == 2)
                {
                    result.Codigo = 200;
                    result.Resultado = true;
                    result.Descripcion = "La cuenta de correo electrónico ha sido actualizada exitosamente.";
                    return result;
                }
                if (_CodigoSalida == 3)
                {
                    result.Codigo = 200;
                    result.Resultado = true;
                    result.Descripcion = "La cuenta de correo electrónico ha sido registrada exitosamente.";
                    return result;
                }
                if (_CodigoSalida == 4)
                {
                    result.Codigo = 999;
                    result.Resultado = false;
                    result.Descripcion = "La cuenta de correo electrónico que desea registrar está asociada a otro NIT.";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Codigo = 999;
                result.Descripcion = "No se pudo registrar la configuración del correo en la base de datos. ";
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(ex), LevelMsn.Error, 0);
            }
            finally
            {
                await CloseConexion();
            }
            return result;
        }

        /// <summary>
        /// Actualiza el token de acceso via OAUTH
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="access_token"></param>
        /// <param name="refresh_token"></param>
        /// <param name="expires"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<IRespuesta> ActualizarTokensOauth(string usuario, string access_token, string refresh_token, DateTime expires, ILogAzure log)
        {
            IRespuesta result = _respuesta;
            result.SetMetodo($"{nameof(DataBase)} . {nameof(ActualizarTokensOauth)}");
            try
            {
                await this.Conexion();
                //Actualizar tokens Oauth
                using SqlCommand cmd = new("sp_AGE_ActualizarTokens", _conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@AccessToken", access_token);
                if (refresh_token == null)
                {
                    cmd.Parameters.AddWithValue("@RefreshToken", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@RefreshToken", refresh_token);
                }

                cmd.Parameters.AddWithValue("@TokenAccessExpirationUTC", expires);

                await cmd.ExecuteNonQueryAsync();

            }
            catch (Exception e)
            {
                result.Codigo = 999;
                result.Descripcion = "No se pudo guardar en base de datos los tokens OAUTH. ";
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + "No se pudo guardar en base de datos los tokens OAUTH.Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
            }
            finally
            {
                await CloseConexion();
            }
            return result;
        }

        /// <summary>
        /// Metodo que permite inactivar la configuración de correo 
        /// </summary>
        /// <param name="Nit"></param>
        /// <param name="TipoIdentificacion"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<IRespuesta> InactivarCorreoReceptor(string Nit, string TipoIdentificacion, ILogAzure log)
        {
            IRespuesta result = _respuesta;
            result.SetMetodo($"{nameof(DataBase)} . {nameof(InactivarCorreoReceptor)}");

            try
            {
                await this.Conexion();

                //Obtener el campo antes de ejecutar el sp

                var campoJson = await GetColumnJson(Nit, TipoIdentificacion, log);

                //Aqui modificar
                campoJson.Log.Add($"Se inactiva correo: {DateTime.UtcNow}");

                string json = JsonConvert.SerializeObject(campoJson);

                //Registro la Cuenta Correo
                using SqlCommand cmd = new("sp_AGE_InactivarEmailReceptor", _conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@NumeroIdentificacion", Nit);
                cmd.Parameters.AddWithValue("@TipoIdentificacion", TipoIdentificacion);
                cmd.Parameters.AddWithValue("@JsonData", json);
                cmd.Parameters.Add("@CodSalida", SqlDbType.Int);
                cmd.Parameters["@CodSalida"].Direction = ParameterDirection.Output;
                await cmd.ExecuteNonQueryAsync();

                int _CodigoSalida = (int)cmd.Parameters["@CodSalida"].Value;

                if (_CodigoSalida == 1)
                {
                    result.Codigo = 999;
                    result.Resultado = false;
                    result.Descripcion = "El NIT ingresado no existe.";
                    return result;
                }

                if (_CodigoSalida == 2)
                {
                    result.Codigo = 999;
                    result.Resultado = false;
                    result.Descripcion = "El NIT ingresado no posee correos activos.";
                    return result;
                }
                result.Codigo = 200;
                result.Resultado = true;
                result.Descripcion = string.Format("La configuración de las cuentas de email para el Número de identificación " + Nit + " y Tipo de identificación " + TipoIdentificacion + " fue guardada satisfactoriamente.");
            }
            catch (Exception e)
            {
                result.Codigo = 999;
                result.Descripcion = "No se pudo registrar la configuración del correo en la base de datos. ";
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);

            }
            finally
            {
                await CloseConexion();
            }
            return result;
        }

        /// <summary>
        /// Obtener el campo de logs para ser modificado
        /// </summary>
        /// <param name="nit"></param>
        /// <param name="tipoIdentificacion"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        private async Task<JsonLogs> GetColumnJson(string nit, string tipoIdentificacion, ILogAzure log)
        {
            JsonLogs dataJson = new();
            try
            {

                log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Antes de obtener objeto Json Tabla CuentasCorreosReceptores", LevelMsn.Info);

                string query = @"select ccr.DeletedAccountLog from enterprise e 
                                 inner join CorreosxReceptores cxr on e.id = cxr.IdReceptor
                                 inner join CuentasCorreosReceptores ccr on cxr.IdCuentaCorreoReceptor = ccr.IdCuentaCorreoReceptor
                                 where e.company_id = @NumeroIdentificacion
                                        and e.document_type = @TipoIdentificacion
                                        and e.active = 1
                                        and e.has_reception = 1
                                        and ccr.Activo = 1;";

                using SqlCommand sql = new(query, _conexion);
                sql.Parameters.AddWithValue("@NumeroIdentificacion", nit);
                sql.Parameters.AddWithValue("@TipoIdentificacion", tipoIdentificacion);

                SqlDataReader reader = await sql.ExecuteReaderAsync();

                if (reader.Read())
                {
                    string? json = reader["DeletedAccountLog"].ToString();

                    if (!string.IsNullOrEmpty(json))
                    {
                        dataJson = JsonConvert.DeserializeObject<JsonLogs>(json);
                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name, JsonConvert.SerializeObject(dataJson), LevelMsn.Info);
                    }
                    else
                    {
                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Campo Vacio", LevelMsn.Info);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Exception: {ex.Message}", LevelMsn.Info);
            }
            return dataJson;
        }

        /// <summary>
        /// Metodo que permite consultar la configuración actual de un correo dado un cliente
        /// </summary>
        /// <param name="numeroIdentificacion"></param>
        /// <param name="tipoIdentificacion"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<IRespuestaApiConsultar> ConsultarEmail(string numeroIdentificacion, string tipoIdentificacion, ILogAzure log)
        {
            IRespuestaApiConsultar result = _respuestaApiConsultar;
            try
            {
                await this.Conexion();
                using SqlCommand cmd = new("dbo.sp_AGE_ConsultarEmail", _conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@identificacion", numeroIdentificacion);
                cmd.Parameters.AddWithValue("@tipoident", tipoIdentificacion);
                cmd.Parameters.Add("@CodSalida", SqlDbType.Int);
                cmd.Parameters["@CodSalida"].Direction = ParameterDirection.Output;

                List<IListadoCorreo> lista = new();

                using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                {
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            _listadoCorreo.correo = rdr["Usuario"].ToString()!;
                            _listadoCorreo.servidor = rdr["Servidor"].ToString()!;
                            _listadoCorreo.puerto = rdr["Puerto"].ToString()!;
                            _listadoCorreo.usarSSL = Convert.ToBoolean(rdr["UsarSSL"]);
                            _listadoCorreo.tipoAutenticacion = (TipoAutenticacion)Enum.Parse(typeof(TipoAutenticacion), rdr["TipoAutenticacion"].ToString()!);
                            lista.Add(_listadoCorreo);
                        }
                    }
                }

                int _CodigoSalida = (int)cmd.Parameters["@CodSalida"].Value;

                if (_CodigoSalida == 1)
                {
                    result.Codigo = 999;
                    result.Resultado = false;
                    result.Mensaje = "No se encontró registro para el Número de identificación " + numeroIdentificacion + " y Tipo de identificación " + tipoIdentificacion + ". Por favor verificar los datos ingresados";
                    return result;
                }

                result.Resultado = true;
                result.Mensaje = string.Format("El Receptor " + numeroIdentificacion + " posee correo activo");
                result.ListadoCorreos = lista;
                result.Codigo = 200;

            }
            catch (Exception e)
            {
                result.Codigo = 999;
                result.Mensaje = "No se pudo consultar la configuración del correo. ";
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
                result.Resultado = false;
            }
            finally
            {
                await CloseConexion();
            }
            return result;
        }

        /// <summary>
        /// Metodo que permite almacenar el tenanId de cuentas con OAUTH
        /// </summary>
        /// <param name="id_receptor"></param>
        /// <param name="email"></param>
        /// <param name="tenantID"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<IRespuesta> RegistrarTenantID(int id_receptor, string email, string tenantID, ILogAzure log)
        {
            IRespuesta result = _respuesta;
            result.SetMetodo($"{nameof(DataBase)} . {nameof(RegistrarTenantID)}");
            try
            {
                await this.Conexion();

                using SqlCommand cmd = new("sp_AGE_GuardarTenantID", _conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id_receptor", id_receptor);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@tenantID", tenantID);
                cmd.Parameters.Add("@CodSalida", SqlDbType.Int);
                cmd.Parameters["@CodSalida"].Direction = ParameterDirection.Output;
                await cmd.ExecuteNonQueryAsync();

                int _CodigoSalida = (int)cmd.Parameters["@CodSalida"].Value;

                if (_CodigoSalida == 0)
                {
                    result.Codigo = 200;
                    result.Resultado = true;
                    result.Descripcion = string.Format("Se está configurando la cuenta de correo <b>\"{0}\"</b> como el buzón de recepción." +
                        "<br><b>Para completar el proceso</b>, asegúrese de realizar los pasos del botón <b>Siguientes pasos</b>, y luego de terminar, presione <b>Probar conexión</b>.", email);
                    return result;
                }

                if (_CodigoSalida != 0) /* El email no está asociado a ese Receptor */
                {
                    result.Codigo = 999;
                    result.Resultado = false;
                    result.Descripcion = "El email " + email + " no está asociado al receptor con ID enterprise o receptor " + id_receptor;
                    return result;
                }

            }
            catch (Exception e)
            {
                result.Codigo = 999;
                result.Descripcion = string.Format("No se pudo guardar en base de datos el TenantId para el receptor {0}", id_receptor);
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + result.Descripcion, log.ConvertToJson(e), LevelMsn.Error, 0);
            }
            finally
            {
                await CloseConexion();
            }
            return result;
        }

        /// <summary>
        /// Metodo que permite obtener el tipo de autenticación de las cuentas OAUTH
        /// </summary>
        /// <param name="id_receptor"></param>
        /// <param name="email"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<IRespuesta> ConsultarTipoAutenticacion(int id_receptor, string email, ILogAzure log)
        {
            IRespuesta result = _respuesta;
            result.SetMetodo($"{nameof(DataBase)} . {nameof(RegistrarTenantID)}");
            try
            {
                await this.Conexion();

                using SqlCommand cmd = new("sp_AGE_ConsultarTipoAutenticacion", _conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id_receptor", id_receptor);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.Add("@CodSalida", SqlDbType.Int);
                cmd.Parameters["@CodSalida"].Direction = ParameterDirection.Output;

                using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                {
                    if (rdr.HasRows)
                    {
                        if (rdr.Read())
                        {
                            result.ValorString = rdr["company_id"].ToString();
                        }
                    }
                }
                int _CodigoSalida = (int)cmd.Parameters["@CodSalida"].Value;

                if (_CodigoSalida == 0)
                {
                    result.Codigo = 200;
                    result.Resultado = true;
                    result.Descripcion = "Ok";
                    return result;
                }

                if (_CodigoSalida != 0) /* No se encontró ese email */
                {
                    result.Codigo = 999;
                    result.Resultado = false;
                    result.Descripcion = "El email " + email + " no está asociado al receptor con ID enterprise o receptor " + id_receptor;
                    return result;
                }
            }
            catch (Exception e)
            {
                result.Codigo = 999;
                result.Descripcion = string.Format("No se pudo consultar el Tipo de Autenticación del email {0} para el receptor {1}", email, id_receptor);
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + result.Descripcion, log.ConvertToJson(e), LevelMsn.Error, 0);
            }
            finally
            {
                await CloseConexion();
            }
            return result;
        }

        /// <summary>
        /// Metodo que obtiene de la base de datos las credenciales para correos OAUTH
        /// </summary>
        /// <param name="id_receptor"></param>
        /// <param name="email"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<ICuentaCorreo> ObtenerCredencialesOauth(int id_receptor, string email, ILogAzure log)
        {
            try
            {
                await this.Conexion();
                using SqlCommand cmd = new("dbo.sp_AGE_ConsultarCredencialesOauth", _conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id_receptor", id_receptor);
                cmd.Parameters.AddWithValue("@email", email);

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    Dictionary<string, object> row = new();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }
                    CuentaCorreo obj = _objectConversion.FromJson(JsonConvert.SerializeObject(row));
                    return obj;
                }
            }

            catch (Exception)
            {
                return null;

            }
            finally
            {
                await CloseConexion();
            }
            return null;
        }

        /// <summary>
        /// Metodo que permite agregar los errores en la tabla invoice_reception_errors
        /// </summary>
        /// <param name="parametros"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task<IRespuesta> RegistrarInvoiceReceptionErrors(InvoiceReceptionError parametros, ILogAzure log)
        {
            Respuesta result = new(nameof(DataBase) + "." + nameof(RegistrarInvoiceReceptionErrors));
            try
            {
                await this.Conexion();
                using SqlCommand cmd = new("sp_insert_invoice_reception_errors", _conexion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id_enterprise", parametros.IdEnterprise);
                cmd.Parameters.AddWithValue("@date_reception", parametros.DateReception);
                cmd.Parameters.AddWithValue("@code", parametros.Code);
                cmd.Parameters.AddWithValue("@message", parametros.Message);
                cmd.Parameters.AddWithValue("@scheme_id", parametros.SchemeId);
                cmd.Parameters.AddWithValue("@party_identification_id", parametros.PartyIdentificationId);
                cmd.Parameters.AddWithValue("@issue_date", parametros.IssueDate);
                cmd.Parameters.AddWithValue("@issue_time", parametros.IssueTime);
                cmd.Parameters.AddWithValue("@document_type", parametros.DocumentType);
                cmd.Parameters.AddWithValue("@document_id", parametros.DocumentId);
                cmd.Parameters.AddWithValue("@mount_total", parametros.MountTotal);
                cmd.Parameters.AddWithValue("@created_at", parametros.CreatedAt);
                cmd.Parameters.AddWithValue("@updated_at", parametros.UpdatedAt);
                cmd.Parameters.AddWithValue("@ubl_version_id", parametros.UblVersionId);
                int resultado = await cmd.ExecuteNonQueryAsync();

                if (resultado == -1)
                {
                    result.Resultado = true;
                    result.Descripcion = "Registrado exitosamente";
                    result.Codigo = 200;
                    result.Detalles = "Proceso de registro de errores se realizo con exito!";
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Se guarda registro de error de carga base de datos", LevelMsn.Error, 0);
                }
            }
            catch (Exception e)
            {
                result.Codigo = 999;
                result.Descripcion = "No se pudo registrar el error del documento en la base de datos. ";
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + ".Exception", log.ConvertToJson(e), LevelMsn.Error, 0);
            }
            finally
            {
                await CloseConexion();
            }
            return (result);
        }

        
    }
}
