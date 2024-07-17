using Domain.Exceptions;
using Domain.Interfaces;
using Domain.TokenContext;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Infrastructure.Helpers
{
    public class ContextHelper(IHttpContextAccessor _httContext ) : IContextHelper
    {
        /// <summary>
        /// Metodo para extraer la información contenida en el token
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GeneralException"></exception>
        public CustomJwtTokenContext TokenContext()
        {
            string contexto;
            try
            {
                if (_httContext.HttpContext != null)
                {
                    contexto = _httContext.HttpContext.User.FindFirst("context")!.Value;
                    TokenSerializable? infotoken = JsonConvert.DeserializeObject<TokenSerializable>(contexto);

                    if (infotoken != null)
                    {
                        return infotoken.User!;
                    }else
                    {
                        return new CustomJwtTokenContext();
                    }
                }
                else
                {
                    return new CustomJwtTokenContext();
                }
            }catch(Exception ex) {
                throw new GeneralException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtener la url de jecución del metodo actual, enrutado via controlador
        /// </summary>
        /// <returns></returns>
        public string UrlContext()
        {
            try
            {
                if (_httContext.HttpContext != null)
                {
                    return _httContext.HttpContext.Request.Path;
                }
                else
                {
                    return "";
                }
            }
            catch(Exception ex)
            {
                throw new GeneralException(ex.Message, ex);
            }
        }
    }
}
