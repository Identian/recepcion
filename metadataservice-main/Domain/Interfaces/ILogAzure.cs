using Domain.Enums;
using Domain.TokenContext;
using System.Diagnostics;

namespace Domain.Interfaces
{
    public interface ILogAzure
    {
        void Savelog(string codigo, string comment, ref Stopwatch timeA, LevelMsn level = LevelMsn.Info, string documentId = null);
        void SetConfig(CustomJwtTokenContext context, string pnameMethod, ApplicationType application, string api);
        void WriteComment(string pname, string pcommnet, LevelMsn plevel = LevelMsn.Info, double timeElapse = 0);
    }
}