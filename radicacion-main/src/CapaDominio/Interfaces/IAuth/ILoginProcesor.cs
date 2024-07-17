namespace CapaDominio.Interfaces.IAuth
{
    public interface ILoginProcesor
    {
        Task<ILoginResponse> Login(ILoginSoap login);
    }
}