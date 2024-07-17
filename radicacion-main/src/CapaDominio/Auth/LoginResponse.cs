using CapaDominio.Interfaces.IAuth;

namespace CapaDominio.Auth
{
    public class LoginResponse : ILoginResponse
    {
        public string? token { get; set; }
        public string? passwordExpiration { get; set; }
    }
}
