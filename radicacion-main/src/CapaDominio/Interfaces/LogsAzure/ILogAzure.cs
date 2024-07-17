using CapaDominio.Enums.Logs;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace CapaDominio.Interfaces.LogsAzure
{
    public interface ILogAzure
    {
        string ConvertToJson(object objtoConvert);
        void SaveLog(string codigo, string comment, string NumIdentificador, string NameMethod, string DocName, string email, ref Stopwatch timeA, string level = "notice", byte[]? document = null);
        void WriteComment(string pname, string pcomment, LevelMsn pLevel = LevelMsn.Info, double timeElapse = 0);
        void SetConfig(IConfiguration configuration, string id);
    }
}