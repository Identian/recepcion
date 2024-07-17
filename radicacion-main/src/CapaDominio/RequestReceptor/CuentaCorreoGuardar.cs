namespace CapaDominio.RequestReceptor
{
    public class CuentaCorreoGuardar : CuentaCorreo
    {
        public string? Nit { get; set; }
        public string? TipoIdentificadorReceptor { get; set; }
        public string? Expires_in { get; set; }
        public string? Ext_expires_in { get; set; }
    }
}
