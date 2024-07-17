namespace CapaDominio.Response
{
    public interface IResponseGeneral
    {
        string? access_token { get; set; }
        string? expires_in { get; set; }
        string? ext_expires_in { get; set; }
        string? refresh_token { get; set; }
        string? scope { get; set; }
        string? token_type { get; set; }
    }
}