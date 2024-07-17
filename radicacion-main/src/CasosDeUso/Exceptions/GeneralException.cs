namespace CasosDeUso.Exceptions
{
    public class GeneralException : Exception
    {
        public string? Details { get; set; }
        public GeneralException() { }
        public GeneralException(string message) : base(message) { }
        public GeneralException(string message, Exception exception) : base(message, exception) { }
        public GeneralException(string title, string details) : base() => Details = details;
    }
}
