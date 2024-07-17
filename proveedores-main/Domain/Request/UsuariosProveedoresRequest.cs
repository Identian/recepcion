
namespace Domain.Request
{
    public class UsuariosProveedoresRequest
    {
        public int AplicationUser {  get; set; }
        public int IdReceptor { get; set; }

        public List<ProvidersRequest> ProvidersRequests { get; set; } = [];
    }
}
