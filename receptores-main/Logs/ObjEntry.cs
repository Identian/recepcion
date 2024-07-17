using TFHKA.AzureStorageLibrary.Loggin;

namespace Receptores.Logs
{
    public class ObjEntry : LogEntry
    {
        public ObjEntry() : base()
        {

        }

        public ObjEntry(string zoneTime) : base(zoneTime)
        {
            PartitionKey = zoneTime;
        }

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
        public string? IpAddress { get; set; }

        public void ClearEntry()
        {
            this.API = "";
            this.EMail = "";
            this.ElapsedTime = 0;
            this.Session = "";
            this.Codigo = 0;
            this.Comment = "";
            this.Document_id = "";
            this.NumIdentificador = "";
            this.Version = "";
            this.PathFile = "";
            this.Process = "";
            this.IpAddress = "";
            this.DocName = "";
        }
    }

}
