namespace CapaDominio.Interfaces.IReceptores
{
    public interface IReceptor
    {
        int Activo { get; set; }
        DateTime? FechaActualizacion { get; set; }
        DateTime? FechaRegistro { get; set; }
        int IdReceptor { get; set; }
        string? NumeroIdentificacionReceptor { get; set; }
        string? TipoIdentificacionReceptor { get; set; }
        string? TokenEnterprise { get; set; }
        string? TokenPassword { get; set; }
    }
}