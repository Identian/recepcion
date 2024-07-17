namespace CapaDominio.Response
{
    public class Respuesta : RespuestaParams, IRespuesta
    {
        private string GetInfoError()
        {
            string result = "";
            if (!Resultado)
            {
                result = (Detalles != "") ? SaltosDeLinea + Detalles : "";
                result += (DetallesAdicionales != "") ? SaltosDeLinea + DetallesAdicionales : "";
            }
            return (result);
        }

        public string GetInfoCompleta(bool omitirSaltosDeLinea = false)
        {
            string result = Metodo + "|" + Descripcion + GetInfoError();
            if (omitirSaltosDeLinea)
            {
                result = result.Replace(SaltosDeLinea, "|").Replace(Environment.NewLine, "|").Replace("||", "|");
            }
            return (result);
        }

        public void Clear()
        {
            IdEntrada = 0;
            IdEntrada = 0;
            Metodo = "";
            Resultado = false;
            Codigo = 0;
            ValorString = "";
            ValorObject = new List<object>();
            ValorObject.Clear();
            Descripcion = "";
            Detalles = "";
            DetallesAdicionales = "";
        }

        public Respuesta() { }

        public Respuesta(string metodo)
        {
            Clear();
            Metodo = metodo;
        }

        public void SetMetodo(string metodo)
        {
            Clear();
            Metodo = metodo;
        }
    }
}
