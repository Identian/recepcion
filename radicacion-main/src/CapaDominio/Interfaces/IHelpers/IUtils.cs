using System.Text;

namespace CapaDominio.Interfaces.IHelpers
{
    public interface IUtils
    {
        public static string Encriptar(string cadenaAencriptar)
        {
            string result = string.Empty;

            try
            {
                byte[] encryted = Encoding.Unicode.GetBytes(cadenaAencriptar);
                result = Convert.ToBase64String(encryted);

            }
            catch (Exception)
            {
                //
            }
            return result;
        }

        /// Esta función desencripta la cadena que le envíamos en el parámentro de entrada.
        public static string DesEncriptar(string cadenaAdesencriptar)
        {
            string result = string.Empty;
            try
            {
                byte[] decryted = Convert.FromBase64String(cadenaAdesencriptar);
                result = Encoding.Unicode.GetString(decryted);
            }
            catch (Exception)
            {
                //
            }

            return result;
        }

        /// <summary>
        /// Obtener el dominio de un correo electronico
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string GetDomain(string email)
        {
            string domain = string.Empty;
            if (!string.IsNullOrEmpty(email))
            {
                int index = email.LastIndexOf("@");
                if (index >= 0)
                {
                    domain = email.Substring(index + 1);
                }
            }
            return domain;
        }

        public static bool TipoIdentificacion(string tipo)
        {
            IDictionary<string, int> tipoId = new Dictionary<string, int>() {
                { "Registro civil", 11 },
                { "Tarjeta de identidad", 12 },
                { "Cedula de ciudadanía", 13 },
                { "Tarjeta de extranjería", 21 },
                { "Cédula de extranjería", 22 },
                { "NIT", 31 },
                { "Pasaporte", 41 },
                { "Documento de identificación extranjero", 42 },
                { "PEP", 47 },
                { "NIT de otro país", 50 },
                { "NUIP*", 91 }
            };
            return tipoId.Values.Any(v => v == int.Parse(tipo));
        }
    }
}
