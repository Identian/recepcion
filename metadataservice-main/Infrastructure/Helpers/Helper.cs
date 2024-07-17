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

        /// <summary>
        /// Metodo para validar el tipo de metadata
        /// </summary>
        /// <param name="format_type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ValidateFormatTypeMetadata(int format_type, string value)
        {
            Match match;

            switch (format_type)
            {
                case 1:
                    return true;
                case 2:
                    match = Regex.Match(value, "^(\\+|-)?\\d+$");
                    break;
                case 3:
                    match = Regex.Match(value, "^[-+]?((\\d{1,3}(,\\d{3})*)|(\\d*))(\\.|\\.\\d*)?$");
                    if (match.Success)
                    {
                        match = Regex.Match(value, "[^.]$");
                    }
                    break;
                case 4:
                    Boolean br = (DateTime.TryParse(value, out DateTime dt) && value.Length <= 10);
                    return br;
                case 5:
                    br = (DateTime.TryParse(value, out dt) && value.Length > 10);
                    return br;

                default: return false;
            }
            return match.Success;
        }
    }
}
