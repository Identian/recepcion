using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IHelper
    {
        /// <summary>
        /// Obtener la fecha y hora de la zona horaria de Colombia
        /// </summary>
        /// <returns></returns>
        DateTime ColombiaTimeZone();
        /// <summary>
        /// Obtener la IP desde donde se esta ejecutando
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GeneralException"></exception>
        string GetLocalIpAddress();
    }
}
