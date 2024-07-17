namespace CapaDominio.Radicacion
{
    public class ArchivoYReceptor
    {
        public int Id { get; set; }
        public int IdReceptor { get; set; }
        public string? Ruta { get; set; }
        public string? DocElectronico { get; set; }
        public string? RepGraf { get; set; }
        public string? Adjunto { get; set; }
        public string? DocElectronicoRecibido { get; set; }
        public string? nitEmisor { get; set; }
        public string? TipoIdentificacionEmisor { get; set; }
        public string? NumDocumento { get; set; }
        public string? Codigo { get; set; }
        public string? NumeroIdentificacionReceptor { get; set; }
        public string? TipoIdentificacionReceptor { get; set; }
        public string? TokenEnterprise { get; set; }
        public string? TokenPassword { get; set; }
        public int Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string? usuario { get; set; }

        public string? NombreXml { get; set; }
        public string? NombreRg { get; set; }
        public string? NombreAdjunto { get; set; }

        public string? TipoDocumento { get; set; }
    }
}
