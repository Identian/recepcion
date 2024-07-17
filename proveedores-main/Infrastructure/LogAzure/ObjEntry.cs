using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFHKA.AzureStorageLibrary.Loggin;

namespace Infrastructure.LogAzure
{
    public class ObjEntry : LogEntry
    {
        public string NameMethod { get; set; } = null!;
        public string NITSolicitante { get; set; } = null!;
        public string NitEmisor { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public double ElapsedTime { get; set; }
        public string Session { get; set; } = null!;
        public string Codigo { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public string DocumentId { get; set; } = null!;
        public string Version { get; set; } = null!;
        public string PathFile { get; set; } = null!;
        public string IpAddress { get; set; } = null!;
        public string Application { get; set; } = null!;
        public string Request { get; set; } = null!;
        public string XmlLog { get; set; } = null!;
        public string Api { get; set; } = null!;

        public ObjEntry() : base() { }
        public ObjEntry(string TimeZone) : base(TimeZone) { }
    }
}
