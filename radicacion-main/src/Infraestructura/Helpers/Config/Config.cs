namespace Infraestructura.Helpers.Config
{
    public static class Config
    {
        public static string GetUsuarioCorreo(string usuario)
        {
            string result = usuario.ToLower();
            int inicio = result.LastIndexOf('<');
            int fin = result.LastIndexOf('>');
            if (inicio >= 0 && fin > inicio)
            {
                result = result.Substring(inicio + 1, fin - inicio - 1);
            }
            return result.Trim();
        }

        public static List<string> GetListaUsuariosCorreo(string usuarios)
        {
            List<string> result = new List<string>();
            foreach (string usuario in usuarios.Split(','))
            {
                result.Add(GetUsuarioCorreo(usuario));
            }
            return result;
        }
    }
}
