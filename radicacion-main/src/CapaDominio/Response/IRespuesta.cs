namespace CapaDominio.Response
{
    public interface IRespuesta : IRespuestaParams
    {
        void Clear();
        string GetInfoCompleta(bool omitirSaltosDeLinea = false);
        void SetMetodo(string metodo);
    }
}