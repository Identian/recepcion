namespace CapaDominio.Interfaces.IAuth
{
    public interface ILoginService
    {
        ILoginResponse LoadLogin(string user, string password);
    }
}