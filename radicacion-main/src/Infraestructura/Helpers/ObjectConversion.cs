using CapaDominio.Interfaces.IHelpers;
using Newtonsoft.Json;

namespace Infraestructura.Helpers
{
    public class ObjectConversion<T> : IObjectConversion<T>
    {
        public T DobleComilla(T o)
        {
            return FromJson(ToJson(o).Replace("'", "''"));
        }

        public T FromJson(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string ToJson(T o)
        {
            string json;
            json = JsonConvert.SerializeObject(o).Replace(":null", ":\"\"");   //"" es null
            return json;
        }
    }
}
