namespace CapaDominio.LogsAzure
{
    public class ObjEntryParams : IObjEntryParams
    {
        public string? API { get; set; }
        public string? EMail { get; set; }
        public double ElapsedTime { get; set; }
        public string? Session { get; set; }
        public int Codigo { get; set; }
        public string? Comment { get; set; }
        public string? Document_id { get; set; }
        public string? NumIdentificador { get; set; }
        public string? DocName { get; set; }
        public string? Version { get; set; }
        public string? PathFile { get; set; }
        public byte[]? xmlreceive { get; set; }
        public string? IpAddress { get; set; }
    }
}
