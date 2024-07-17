namespace CapaDominio.Interfaces.IAuth
{
    public interface ILoginResponse
    {
        string? passwordExpiration { get; set; }
        string? token { get; set; }
    }
}