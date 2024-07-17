using Newtonsoft.Json;
using Receptores.Interfaces;
using Receptores.Logs;
using Receptores.Model.Estructuras;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Receptores.Services
{
    public class ProcesarCuentas : IProcesarCuentas
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabaseQueries _databaseQueries;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ProcesarCuentas> _logger;

        public ProcesarCuentas(IConfiguration configuration, IDatabaseQueries databaseQueries, IHttpClientFactory clientFactory, ILogger<ProcesarCuentas> logger)
        {
            _configuration = configuration;
            _databaseQueries = databaseQueries;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Metodo de encargado de recibir y enviar la información del receptor 
        /// Al servicio de Radicación
        /// </summary>
        /// <param name="cuenta"></param>
        /// <returns></returns>
        public async Task ProcesarMensajes(Estructura cuenta)
        {
            int correosAProcesar = _configuration.GetValue<int>("MaxCorreosAProcesar");
            int tiempoMaximo = _configuration.GetValue<int>("TiempoMaximoProcesoPorCuenta", 480000);

            LogAzure log = new(cuenta.GetIdReceptor().ToString(), _configuration);

            int correosProcesadosTotal = 0;
            Stopwatch time = new();
            int codeResponse = 0;
            if (cuenta != null)
            {
                try
                {
                    time.Start();

                    HttpClient client = _clientFactory.CreateClient("Radicacion");
                    System.Timers.Timer timer = new(tiempoMaximo)
                    {
                        AutoReset = false
                    };

                    timer.Elapsed += (sender, e) =>
                    {
                        // Cuando el tiempo límite se alcance, aquí detendremos el bucle y saldremos de él
                        log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Tiempo maximo de procesamiento por cuenta alcanzado {correosProcesadosTotal}", LevelMsn.Info, 0);
                        timer.Stop();
                    };
                    timer.Start();

                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Procesando cuenta" + cuenta.GetUsuario()!, LevelMsn.Info, 0);

                    int flag = 0;
                    while (correosProcesadosTotal < correosAProcesar)
                    {

                        var body = new { cb = cuenta.GetCuentaCorreoReceptorBase(), r = cuenta.GetReceptor() };

                        string jsonData = JsonConvert.SerializeObject(body);
                        StringContent header = new(jsonData, encoding: Encoding.UTF8, "application/json");

                        HttpResponseMessage request = await client.PostAsync("", header);

                        codeResponse = (int)request.EnsureSuccessStatusCode().StatusCode;

                        string response = await request.Content.ReadAsStringAsync();

                        int correosProcesados = int.TryParse(response, out int value) ? value : 0;
                        Interlocked.Add(ref correosProcesadosTotal, correosProcesados);

                        if (!timer.Enabled)
                        {
                            log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Proceso terminado por timeOut = {codeResponse} correos leidos {correosProcesadosTotal}", LevelMsn.Info, 0);
                            await _databaseQueries.UpdateAccounts(1, cuenta.GetIdCuentaCorreo());
                            break;
                        }

                        if (correosProcesados != 0 && flag == 0)
                        {
                            flag++;
                            await _databaseQueries.UpdateAccounts(2, cuenta.GetIdCuentaCorreo());
                            log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Procesando cuenta = {cuenta.GetUsuario()!} ", LevelMsn.Info, 0);
                        }

                        if (correosProcesados == 0)
                        {
                            log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Cuenta procesada = {request.StatusCode} correos leidos {correosProcesadosTotal}", LevelMsn.Info, 0);
                            await _databaseQueries.UpdateAccounts(1, cuenta.GetIdCuentaCorreo());
                            break;
                        }

                        if (correosProcesados >= correosAProcesar)
                        {
                            log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Correos procesados {correosProcesadosTotal}", LevelMsn.Info, 0);
                            await _databaseQueries.UpdateAccounts(1, cuenta.GetIdCuentaCorreo());
                            break;
                        }
                    }

                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name, $"Cuenta {cuenta.GetUsuario()!} se procesaron un total de {correosProcesadosTotal} correos", LevelMsn.Info, 0);
                }
                catch (Exception ex)
                {
                    time.Start();
                    await _databaseQueries.UpdateAccounts(1, cuenta.GetIdCuentaCorreo());
                    log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Error servicio de gestion de correo no localizado " + ex.Message + " -------------- ", LevelMsn.Error, 0);
                    log.SaveLog($"{codeResponse}", cuenta.GetIdCuentaCorreo().ToString(), "Datos de correo -- ERROR", cuenta.GetNumIdentificacionReceptor()!, "AgeReceptores", "", cuenta.GetUsuario()!, ref time);
                }
                finally
                {
                    _logger.LogWarning("Fin proceso {cuenta} correos procesados {correos}", cuenta.GetUsuario(), correosProcesadosTotal);
                    await _databaseQueries.UpdateAccounts(1, cuenta.GetIdCuentaCorreo());
                    log.SaveLog($"{codeResponse}", cuenta.GetIdCuentaCorreo().ToString(), $"Finalizo el procesamiento cuenta {cuenta.GetUsuario()}", cuenta.GetNumIdentificacionReceptor()!, "AgeReceptores", "", cuenta.GetUsuario()!, ref time);
                }
            }
        }
    }
}
