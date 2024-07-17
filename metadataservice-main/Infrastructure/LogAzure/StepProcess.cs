using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LogAzure
{
    public class StepProcess
    {
        public double TimeElapse { get; set; }
        public DateTime HoraProcess { get; set; }
        public string NameProcess { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public string LevelInfo { get; set; } = null!;
        public Enum LevelMsn { get; set; } = null!;
    }
}
