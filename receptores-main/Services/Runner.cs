using Receptores.Interfaces;
using Receptores.Interfaces.IRunners;
using Receptores.Logs;
using Receptores.Model.Estructuras;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks.Dataflow;

namespace Receptores.Services
{
    public class Runner : IRunner
    {
        private readonly IDatabaseQueries _database;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Runner> _logger;
        private readonly IProcesarCuentas _procesarCuentas;

        public Runner(IDatabaseQueries database, IConfiguration configuration, ILogger<Runner> logger, IProcesarCuentas procesarCuentas)
        {
            _database = database;
            _configuration = configuration;
            _logger = logger;
            _procesarCuentas = procesarCuentas;
        }

        /// <summary>
        /// Metodo encargado de obtener las cuentas de los receptores y realizar el envio 
        /// A Radicación para su procesamiento.
        /// </summary>
        /// <returns>Task<long></returns>
        public async Task<long> ExcuteRunner()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();


            



            List<Model.Estructuras.Estructura> listaDeCuentas = await _database.GetAccounts();

            int currenMaxThreads = _configuration.GetValue<int>("CantidadDeHilosMaxima");

            _logger.LogInformation("******** Inicia proceso de lectura ********");

            try
            {
                BufferBlock<Estructura> buffer = new(new GroupingDataflowBlockOptions { BoundedCapacity = currenMaxThreads });
                ActionBlock<Estructura> actionBlock = new(async cuenta =>
                {
                    try
                    {
                        await _procesarCuentas.ProcesarMensajes(cuenta);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = currenMaxThreads, BoundedCapacity = currenMaxThreads });

                buffer.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });
                foreach (Estructura estructura in listaDeCuentas)
                {
                    await buffer.SendAsync(estructura);
                }
                buffer.Complete();
                await actionBlock.Completion;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error: Exception: {exception}", ex.Message);
                Stopwatch timeT = new();
                LogAzure log = new();
                timeT.Start();
                log.WriteComment(MethodBase.GetCurrentMethod()!.Name, "Error en el proceso de busqueda de correos: " + ex.Message + " -------------- " + ex.StackTrace, LevelMsn.Error, 0);
                log.SaveLog("0", "500", "Datos de correo -- ERROR", "", "AgeReceptores", "", "", ref timeT);
            }

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
