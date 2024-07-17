namespace CapaDominio.Interfaces.IAuth
{
    public interface ILoginSoap
    {
        string? password { get; set; }
        string? user { get; set; }
    }
}