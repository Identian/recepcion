namespace CapaDominio.Interfaces.IReceptores
{
    public interface IReceptorBaseParams
    {
        int IdReceptor { get; set; }
        string? NumeroIdentificacionReceptor { get; set; }
        string? ServicioRecepcion { get; set; }
        string? TipoIdentificacionReceptor { get; set; }
        string? TokenEnterprise { get; set; }
        string? TokenPassword { get; set; }
        DateTime UltimaActualizacion { get; set; }
    }
}