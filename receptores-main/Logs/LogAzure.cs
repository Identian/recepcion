using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using LogLevel = TFHKA.AzureStorageLibrary.Loggin.LogLevel;
namespace Receptores.Logs
{
    public class LogAzure : IDisposable
    {
        private string? _accountName;
        private string? _accountKey;
        private string? _appId;
        private string? _level;
        private string? _zoneTime;
        private TFHKA.AzureStorageLibrary.Loggin.Logger<ObjEntry>? _log;
        private ObjEntry? _entryLog;
        private readonly List<StepProcess> _pasos;

        private string? _levelLog;

        private readonly IConfiguration? _configuration;

        public LogAzure()
        {
            _pasos = new List<StepProcess>();
            _entryLog = new ObjEntry();
            LoadConfig();
            _log = new TFHKA.AzureStorageLibrary.Loggin.Logger<ObjEntry>(_appId, _accountName, _accountKey, "1", level: _level);
        }

        public LogAzure(string id, IConfiguration configuration)
        {
            this._configuration = configuration;
            _pasos = new List<StepProcess>();
            _entryLog = new ObjEntry(id);
            LoadConfig();
            _log = new TFHKA.AzureStorageLibrary.Loggin.Logger<ObjEntry>(_appId, _accountName, _accountKey, id, level: _level);
        }

        public LogAzure(string ptokenEnterprise, string pnameMethod, string pSession)
        {
            string? partitionKey;
            try
            {
                byte[] byteString = System.Text.Encoding.UTF8.GetBytes(ptokenEnterprise);
                partitionKey = Convert.ToBase64String(byteString);

            }
            catch (Exception)
            {
                partitionKey = "errorBase64";
            }

            _pasos = new List<StepProcess>();
            LoadConfig();
            _entryLog = new(_zoneTime ?? "")
            {
                RowKeyTime = ColombiaTimezone(),
                PartitionKey = partitionKey,
                API = pnameMethod,
                Session = pSession
            };
            _log = new TFHKA.AzureStorageLibrary.Loggin.Logger<ObjEntry>(_appId, _accountName, _accountKey, partitionKey, level: _level);
        }

        private void LoadConfig()
        {
            _accountName = _configuration!.GetValue<string>("Azure:AccountName", "") ?? "";
            _accountKey = _configuration!.GetValue<string>("Azure:AccountKey", "") ?? "";
            DateTime peru = ColombiaTimezone();
            _appId = _configuration!.GetValue<string>("AppId", "") + peru.ToString("yyyyMMdd");
            _zoneTime = _configuration!.GetValue<string>("TimeZoneColombia", "") ?? "";
            string readLevel = _configuration!.GetValue<string>("Azure:LogLevel", "Error") ?? "Error";

            _level = LogLevel.Notice;
            switch (readLevel)
            {
                case "Error":
                    _levelLog = LogLevel.Error;
                    break;
                case "Notice":
                    _levelLog = LogLevel.Notice;
                    break;
                case "Off":
                    _levelLog = LogLevel.Off;
                    break;
                case "Warning":
                    _levelLog = LogLevel.Error;
                    break;

            }

        }

        public void WriteComment(string pname, string pcomment, LevelMsn pLevel = LevelMsn.Info, double timeElapse = 0)
        {
            try
            {
                if (_levelLog!.Equals("error") && _levelLog.Equals(pLevel.ToString().ToLower()))
                {
                    _pasos.Add(new StepProcess { HoraProcess = ColombiaTimezone(), NameProcess = pname, Comment = pcomment, LevelInfo = pLevel.ToString(), TimeElapse = timeElapse });
                }
                else
                {
                    if (_levelLog.Equals("warning") && (_levelLog.Equals(pLevel.ToString().ToLower()) || (pLevel.ToString().Equals("Error"))))
                    {
                        _pasos.Add(new StepProcess { HoraProcess = ColombiaTimezone(), NameProcess = pname, Comment = pcomment, LevelInfo = pLevel.ToString(), TimeElapse = timeElapse });
                    }
                    else
                    {
                        if (_levelLog.Equals("notice") && (pLevel.ToString().Equals("Info") || (pLevel.ToString().Equals("Error")) || (pLevel.ToString().Equals("Warning"))))
                        {
                            _pasos.Add(new StepProcess { HoraProcess = ColombiaTimezone(), NameProcess = pname, Comment = pcomment, LevelInfo = pLevel.ToString(), TimeElapse = timeElapse });
                        }
                    }
                }
            }
            catch (Exception)
            {
                //Se asume nivel de error
                _pasos.Add(new StepProcess { HoraProcess = ColombiaTimezone(), NameProcess = pname, Comment = pcomment, LevelInfo = pLevel.ToString(), TimeElapse = timeElapse });
            }
        }

        public void SaveLog(string codigo, string Id, string comment, string NumIdentificador, string NameMethod, string DocName, string email, ref Stopwatch timeA, string level = LogLevel.Notice)
        {
            try
            {
                _ = int.TryParse(codigo, out int code);
                timeA.Stop();
                lock (_entryLog!)
                {
                    _entryLog.Process = ConvertToJson(_pasos);
                    _entryLog.Codigo = code;
                    _entryLog.Comment = comment;
                    _entryLog.Level = level;
                    _entryLog.IpAddress = GetLocalIPAddress();
                    _entryLog.NumIdentificador = NumIdentificador;
                    _entryLog.API = NameMethod;
                    _entryLog.DocName = DocName;
                    _entryLog.EMail = email;
                    _entryLog.ElapsedTime = timeA.ElapsedMilliseconds;
                    _log!.Add(_entryLog);
                    _pasos.Clear();
                    _entryLog.ClearEntry();
                }
            }
            catch (Exception)
            {
                _pasos.Clear();
                _entryLog!.ClearEntry();
            }
        }

        public void SaveLog(string codigo, string comment, string NumIdentificador, string NameMethod, string DocName, string email, ref Stopwatch timeA, string level = LogLevel.Notice)
        {
            try
            {
                _ = int.TryParse(codigo, out int code);
                timeA.Stop();
                lock (_entryLog!)
                {
                    _entryLog.Process = ConvertToJson(_pasos);
                    _entryLog.Codigo = code;
                    _entryLog.Comment = comment;
                    _entryLog.Level = level;
                    _entryLog.IpAddress = GetLocalIPAddress();
                    _entryLog.NumIdentificador = NumIdentificador;
                    _entryLog.API = NameMethod;
                    _entryLog.DocName = DocName;
                    _entryLog.EMail = email;
                    _entryLog.ElapsedTime = timeA.ElapsedMilliseconds;
                    _log!.Add(_entryLog);
                    _pasos.Clear();
                    _entryLog.ClearEntry();
                }
            }
            catch (Exception)
            {
                _pasos.Clear();
                _entryLog!.ClearEntry();
            }
        }

        public static string ConvertToJson(object objtoConvert)
        {
            try
            {
                return JsonConvert.SerializeObject(objtoConvert);
            }
            catch (Exception)
            {
                return "Error en ConvertToJson";
            }
        }

        public static string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new InvalidOperationException("No network adapters with an IPv4 address in the system!");
        }

        public DateTime ColombiaTimezone()
        {
            string? time = _configuration!.GetValue<string>("TimeZoneColombia", string.Empty) ?? "";
            TimeZoneInfo? timeZone = TimeZoneInfo.FindSystemTimeZoneById(time);

            DateTime dateColombia = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, timeZone);
            return dateColombia;
        }

        public void Dispose()
        {
            _entryLog = null;
            _pasos.Clear();
            _log = null;
            GC.SuppressFinalize(this);
        }
    }

}
