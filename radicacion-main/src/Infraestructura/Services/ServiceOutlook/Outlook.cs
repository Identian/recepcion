using CapaDominio.Enums.Logs;
using CapaDominio.Enums.TipoAutenticacion;
using CapaDominio.Errors;
using CapaDominio.Interfaces.IDB;
using CapaDominio.Interfaces.IHelpers;
using CapaDominio.Interfaces.IProcesos.Conexion;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Interfaces.ServiceOutlook;
using CapaDominio.Response;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection;

namespace Infraestructura.Services.ServiceOutlook
{
    public class Outlook : IOutlook
    {
        public string access_token { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public DateTime expires_in { get; set; }
        public string grant_type { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string tenant_id { get; set; }
        public TipoAutenticacion tipo_autenticacion { get; set; }
        public string url_token { get; set; }
        public string url_token_domain { get; set; }
        public string url_token_common { get; set; }

        private readonly IDataBase _dataBases;
        private readonly IConfiguration _configuration;
        private readonly IRespuestaApi _respuestaApi;
        private readonly IUtils _utils;

        public Outlook(IDataBase dataBase, IConfiguration configuration, IRespuestaApi respuestaApi, IUtils utils)
        {
            _dataBases = dataBase;
            _configuration = configuration;
            _respuestaApi = respuestaApi;
            _utils = utils;
        }


        public void SetTypeConecction(TipoAutenticacion tipoAutenticacion, string accessToken, string refreshToken, string tenantId, string correo)
        {
            client_id = _configuration.GetSection("Office365:ClientID").Value ?? "";
            client_secret = _configuration.GetSection("Office365:ClientSecret").Value ?? "";

            tipo_autenticacion = tipoAutenticacion;
            access_token = accessToken;

            switch (tipoAutenticacion)
            {
                case TipoAutenticacion.MICROSOFT_OAUTH_CODIGO:
                    grant_type = "refresh_token";
                    refresh_token = refreshToken;
                    scope = "openid offline_access https://outlook.office.com/IMAP.AccessAsUser.All";
                    url_token = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
                    break;
                case TipoAutenticacion.MICROSOFT_OAUTH_APLICACION:
                    string domain = _utils.GetDomain(correo);
                    grant_type = "client_credentials";
                    scope = "https://outlook.office365.com/.default";
                    url_token = "https://login.microsoftonline.com/" + tenantId + "/oauth2/v2.0/token";
                    url_token_domain = $"https://login.microsoftonline.com/{domain}/oauth2/v2.0/token";
                    url_token_common = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
                    break;
            }

        }

        public async Task<IRespuestaApi> GuardarTokens(string usuario, ILogAzure log)
        {
            IRespuesta result = new Respuesta();
            result.SetMetodo(nameof(IProcesoRecepcionConexion) + "." + nameof(GuardarTokens));
            try
            {
                /*Vamos a ajustar los tiempos de expiracion de los tokens*/
                result = await _dataBases.ActualizarTokensOauth(usuario, access_token, refresh_token, expires_in, log);

                _respuestaApi.Codigo = result.Codigo;
                _respuestaApi.Mensaje = result.Descripcion!.ToString();
                return _respuestaApi;

            }
            catch (Exception ex)
            {
                _respuestaApi.Codigo = 500;
                _respuestaApi.Mensaje = ErrorsCodes._500A;

                log.WriteComment(MethodBase.GetCurrentMethod()!.Name + "Guardar Tokens en Base de datos. Exception", log.ConvertToJson(ex), LevelMsn.Error, 0);
                return _respuestaApi;
            }
        }

        public Task<bool> Refresh_tokens(ILogAzure log)
        {
            try
            {
                HttpResponseMessage response;
                Dictionary<string, string> parametros = new()
                {
                    { "client_id", client_id },
                    { "scope", scope },
                    { "grant_type", grant_type },
                    { "client_secret", client_secret }
                };

                if (tipo_autenticacion == TipoAutenticacion.MICROSOFT_OAUTH_CODIGO)
                {
                    parametros.Add("refresh_token", refresh_token);
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    response = client.PostAsync(url_token, new FormUrlEncodedContent(parametros)).Result;

                    bool seguir = false;

                    //Si la primera llamada no es exitosa se reintenta con el dominio
                    if (response.IsSuccessStatusCode)
                    {
                        seguir = true;
                    }
                    else
                    {
                        response = client.PostAsync(url_token_domain, new FormUrlEncodedContent(parametros)).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            seguir = true;
                        }
                        else
                        {
                            response = client.PostAsync(url_token_common, new FormUrlEncodedContent(parametros)).Result;
                            if (response.IsSuccessStatusCode)
                            {
                                seguir = true;
                            }
                        }
                    }

                    if (seguir)
                    {
                        string tokens = response.Content.ReadAsStringAsync().Result;

                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name + " refresh_tokens ", tokens, LevelMsn.Info, 0);

                        IResponseGeneral respuesta = JsonConvert.DeserializeObject<ResponseGeneral>(tokens);

                        access_token = respuesta.access_token is null ? access_token : respuesta.access_token;

                        refresh_token = respuesta.refresh_token is null ? refresh_token : respuesta.refresh_token;

                        expires_in = DateTime.UtcNow.AddMilliseconds(Convert.ToDouble(respuesta.expires_in));
                        return Task.FromResult(true);
                    }
                    else
                    {
                        return Task.FromResult(false);
                    }
                }
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }
    }
}