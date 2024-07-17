using CapaDominio.Auth;
using CapaDominio.Interfaces.IAuth;

namespace Infraestructura.Services.Login
{
    public class LoginService : ILoginService
    {
        private readonly ILoginProcesor _loginProcesor;

        public LoginService(ILoginProcesor loginProcesor)
        {
            _loginProcesor = loginProcesor;
        }

        public ILoginResponse LoadLogin(string user, string password)
        {
            return _loginProcesor.Login(new LoginSoap() { user = user, password = password }).Result;
        }
    }
}
