using Domain.TokenContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IContextHelper
    {
        /// <summary>
        /// Metodo que obtiene la información contenida en el token de usuario
        /// </summary>
        /// <returns></returns>
        CustomJwtTokenContext TokenContext();

        /// <summary>
        /// Obtener las ruta url del contexto del controlador
        /// </summary>
        /// <returns></returns>
        string UrlContext();
    }
}
