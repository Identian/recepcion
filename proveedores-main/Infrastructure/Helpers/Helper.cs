using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Infrastructure.Helpers
{
    public class Helper(IConfiguration _configuration) : IHelper
    {
        /// <summary>
        /// Obtener la fecha y hora de la zona horaria de Colombia
        /// </summary>
        /// <returns></returns>
        public DateTime ColombiaTimeZone()
        {
            string time = _configuration.GetSection("TimeZones:Core.TimeZoneColombia").Value ?? "SA Pacific Standard Time";
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(time);
            DateTime dateColombia = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, timeZone);
            return dateColombia;
        }


        public string GetLocalIpAddress()
        {
            string ipAddress = string.Empty;
            try
            {
                ipAddress = GetLocalIpAddressInternal();
            }
            catch (Exception ex)
            {
                throw new GeneralException(ex.Message, ex);
            }
            return ipAddress;
        }


        /// <summary>
        /// Obtener la IP desde donde se esta ejecutando
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GeneralException"></exception>
        protected virtual string GetLocalIpAddressInternal()
        {
            string ipAddress = string.Empty;
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach(IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddress = ip.ToString();
                    }
                }

            }catch (Exception ex)
            {
                throw new GeneralException(ex.Message, ex);
            }
            return ipAddress;
        }
    }
}
