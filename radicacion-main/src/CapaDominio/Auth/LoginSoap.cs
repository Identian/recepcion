using CapaDominio.Interfaces.IAuth;

namespace CapaDominio.Auth
{
    public class LoginSoap : ILoginSoap
    {
        public string? user { get; set; }
        public string? password { get; set; }
    }
}
