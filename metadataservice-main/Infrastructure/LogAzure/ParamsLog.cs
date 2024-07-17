using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFHKA.AzureStorageLibrary.Loggin;

namespace Infrastructure.LogAzure
{
    public class ParamsLog
    {
        public string AccountName { get; set; } = null!;
        public string AccountKey { get; set; } = null!;
        public string AppId { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string ZoneTime { get; set; } = null!;
        protected Logger<ObjEntry> Log { get; set; } = null!;
        public ObjEntry EntryLog { get; set; } = null!;
        protected List<StepProcess> Pasos { get; set; } = null!;
    }
}
