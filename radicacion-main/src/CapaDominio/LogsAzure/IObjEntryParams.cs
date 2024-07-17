namespace CapaDominio.LogsAzure
{
    public interface IObjEntryParams
    {
        string? API { get; set; }
        int Codigo { get; set; }
        string? Comment { get; set; }
        string? DocName { get; set; }
        string? Document_id { get; set; }
        double ElapsedTime { get; set; }
        string? EMail { get; set; }
        string? IpAddress { get; set; }
        string? NumIdentificador { get; set; }
        string? PathFile { get; set; }
        string? Session { get; set; }
        string? Version { get; set; }
        byte[]? xmlreceive { get; set; }
    }
}