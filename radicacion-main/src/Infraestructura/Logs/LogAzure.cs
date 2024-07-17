using CapaDominio.Enums.Logs;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.LogsAzure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using TFHKA.AzureStorageLibrary.Loggin;

namespace Infraestructura.Logs
{
    public class LogAzure : ILogAzure
    {
        private string? _accountName;
        private string? _accountKey;
        private string? _appId;
        private string? _level;
        private Logger<ObjEntry> _log;
        private ObjEntry _entryLog;
        public List<StepProcess> _pasos;
        private static string? _pathrr;
        private string? _tzc;

        private string? _levelLog;

        private IConfiguration _configuration;

        public LogAzure(IConfiguration configuration)
        {
            _configuration = configuration;
            LoadConfig();
            _pasos = new List<StepProcess>();
            _entryLog = new ObjEntry();
            _log = new Logger<ObjEntry>(_appId, _accountName, _accountKey, "1", level: _level);

        }

        public LogAzure(IConfiguration configuration, string id)
        {
            _configuration = configuration;
            _pasos = new List<StepProcess>();
            _entryLog = new ObjEntry(id);
            LoadConfig();
            _log = new Logger<ObjEntry>(_appId, _accountName, _accountKey, id, level: _level);
        }

        public void SetConfig(IConfiguration configuration, string id)
        {
            _configuration = configuration;
            _pasos = new List<StepProcess>();
            _entryLog = new ObjEntry(id);
            LoadConfig();
            _log = new Logger<ObjEntry>(_appId, _accountName, _accountKey, id, level: _level);
        }


        private void LoadConfig()
        {
            _tzc = _configuration.GetSection("TimeZoneColombia").Value ?? "";
            _accountName = _configuration.GetSection("Azure:AccountName").Value ?? "";
            _accountKey = _configuration.GetSection("Azure:AccountKey").Value ?? "";
            DateTime horaLocal = ColombiaTimezone();
            _appId = _configuration.GetSection("AppId").Value + horaLocal.ToString("yyyyMMdd");
            _pathrr = "";

            string readLevel = _configuration.GetSection("Azure:LogLevel").Value ?? "Error";
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
                    if (_levelLog.Equals("warning") && (_levelLog.Equals(pLevel.ToString().ToLower()) || pLevel.ToString().Equals("Error")))
                    {
                        _pasos.Add(new StepProcess { HoraProcess = ColombiaTimezone(), NameProcess = pname, Comment = pcomment, LevelInfo = pLevel.ToString(), TimeElapse = timeElapse });
                    }
                    else
                    {
                        if (_levelLog.Equals("notice") && (pLevel.ToString().Equals("Info") || pLevel.ToString().Equals("Error") || pLevel.ToString().Equals("Warning")))
                        {
                            _pasos.Add(new StepProcess { HoraProcess = ColombiaTimezone(), NameProcess = pname, Comment = pcomment, LevelInfo = pLevel.ToString(), TimeElapse = timeElapse });
                        }
                    }
                }
            }
            catch (Exception)
            {
                _pasos.Add(new StepProcess { HoraProcess = ColombiaTimezone(), NameProcess = pname, Comment = pcomment, LevelInfo = pLevel.ToString(), TimeElapse = timeElapse });
            }
        }

        public void SaveLog(string codigo, string comment, string NumIdentificador, string NameMethod, string DocName, string email, ref Stopwatch timeA, string level = LogLevel.Notice, byte[]? document = null)
        {
            try
            {
                int.TryParse(codigo, out int code);
                timeA.Stop();
                lock (_entryLog)
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
                    _log.Add(_entryLog);
                    _pasos.Clear();
                    _entryLog.ClearEntry();
                }
            }
            catch (Exception)
            {
                _pasos.Clear();
                _entryLog.ClearEntry();
            }
        }

        public string ConvertToJson(object objtoConvert)
        {
            try
            {
                return JsonConvert.SerializeObject(objtoConvert);
            }
            catch (Exception ex)
            {
                return $"Error en ConvertToJson {ex}";
            }
        }

        private static string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            Exception exception = new("No network adapters with an IPv4 address in the system!");
            throw exception;
        }

        private DateTime ColombiaTimezone()
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(_tzc ?? "");
            DateTime dateColombia = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, timeZone);
            return dateColombia;
        }
    }
}
