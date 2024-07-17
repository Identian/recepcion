namespace CapaDominio.Interfaces.IHelpers
{
    public interface IObjectConversion<T>
    {
        public string ToJson(T o);
        public T FromJson(string json);
        public T DobleComilla(T o);
    }
}
