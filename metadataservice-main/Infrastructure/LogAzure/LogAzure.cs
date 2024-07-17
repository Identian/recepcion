using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.TokenContext;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using TFHKA.AzureStorageLibrary.Loggin;

namespace Infrastructure.LogAzure
{
    public class LogAzure(IHelper _helper, IConfiguration _configuration) : ParamsLog, ILogAzure
    {
        public void SetConfig(CustomJwtTokenContext context, string pnameMethod, ApplicationType application, string api)
        {
            Pasos = [];
            EntryLog = new ObjEntry($"{_helper.ColombiaTimeZone()}")
            {
                RowKeyTime = _helper.ColombiaTimeZone(),
                PartitionKey = context.EnterpriseToken,
                NITSolicitante = context.EnterpriseNit,
                NameMethod = pnameMethod,
                Session = Guid.NewGuid().ToString(),
                Application = application.ToString(),
                Version = $"{Assembly.GetExecutingAssembly().GetName().Version}",
                Api = api
            };


            AccountName = _configuration.GetSection("StorageAzure:AccountName").Value ?? "";
            AccountKey = _configuration.GetSection("StorageAzure:AccountKey").Value ?? "";
            AppId = _configuration.GetSection("StorageAzure:Tablename").Value + DateTime.Now.ToString("yyyyMM");
            ZoneTime = _configuration.GetSection("TimeZones:Core.TimeZoneColombia").Value ?? "";

            string readLevel = _configuration.GetSection("StorageAzure:LogLevel").Value ?? "";

            switch (readLevel)
            {
                case "Error":
                    Level = LogLevel.Error;
                    break;
                case "Notice":
                    Level = LogLevel.Notice;
                    break;
                case "Off":
                    Level = LogLevel.Off;
                    break;
                case "Warning":
                    Level = LogLevel.Error;
                    break;
            }

            Log = new Logger<ObjEntry>(AppId, AccountName, AccountKey, EntryLog.PartitionKey, level: Level);

        }

        /// <summary>
        /// Escribir los pasos del log
        /// </summary>
        /// <param name="pname"></param>
        /// <param name="pcommnet"></param>
        /// <param name="plevel"></param>
        /// <param name="timeElapse"></param>
        /// <exception cref="GeneralException"></exception>
        public void WriteComment(string pname, string pcommnet, LevelMsn plevel = LevelMsn.Info, double timeElapse = 0)
        {
            try
            {
                Pasos.Add(new StepProcess
                {
                    HoraProcess = _helper.ColombiaTimeZone(),
                    NameProcess = pname,
                    Comment = pcommnet,
                    LevelInfo = plevel.ToString(),
                    TimeElapse = timeElapse
                });
            }
            catch (Exception ex)
            {
                throw new GeneralException(ex.Message, ex);
            }
        }

        public void Savelog(string codigo, string comment, ref Stopwatch timeA, LevelMsn level = LevelMsn.Info, string documentId = "")
        {
            try
            {

                timeA.Stop();
                lock (EntryLog)
                {
                    EntryLog.Process = JsonConvert.SerializeObject(CopyStepProcess());
                    EntryLog.Codigo = codigo;
                    EntryLog.Comment = comment;
                    EntryLog.Level = level.ToString();
                    EntryLog.IpAddress = _helper.GetLocalIpAddress();
                    EntryLog.ElapsedTime = timeA.ElapsedMilliseconds;
                    EntryLog.DocumentId = !string.IsNullOrEmpty(documentId) ? documentId.Trim() : "";
                    Log.Add(EntryLog, EntryLog.Level);
                    Pasos.Clear();
                    EntryLog = new ObjEntry();
                }

            }
            catch (Exception ex)
            {
                Pasos.Clear();
                EntryLog = new ObjEntry();
                throw new GeneralException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Metodo que retorna una copia de la lista de pasos para evitar modificar una lista que esta siendo leida
        /// </summary>
        /// <returns></returns>
        private List<StepProcess> CopyStepProcess()
        {
            return new List<StepProcess>(Pasos);
        }

    }
}
