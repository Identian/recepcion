using Receptores.Interfaces;
using Receptores.Interfaces.IRunners;

namespace Receptores
{
    public class Worker
    {
        private readonly IConfiguration _configuration;
        private readonly IRunner _runner;
        private readonly IDatabaseQueries _databaseQueries;
        private readonly ILogger<Worker> _logger;
        private bool _actualizacion = false;

        public Worker(IConfiguration configuration, IRunner runner, IDatabaseQueries databaseQueries, ILogger<Worker> logger)
        {
            _configuration = configuration;
            _runner = runner;
            _databaseQueries = databaseQueries;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            if (!_actualizacion)
            {
                _actualizacion = await _databaseQueries.UpdateAcountsFree();
            }

            int PausaEntreIteraciones = _configuration.GetValue("PausaEntreIteracionesTiempoEnMinutos", 10);
            try
            {

                _logger.LogInformation("{salto}******** Inicio de ciclo ********", Environment.NewLine);


                long tiempoDeEjecucion = await _runner.ExcuteRunner();
                _logger.LogInformation("******** Ciclo finalizado tiempo: {tiempoDeEjecucion}ms {salto} ********", tiempoDeEjecucion, Environment.NewLine);
                long tiempoRestanteParaEsperar = (PausaEntreIteraciones * 60 * 1000) - tiempoDeEjecucion;

                if (tiempoRestanteParaEsperar > 0)
                {
                    _logger.LogInformation("******** Tiempo de espera: {tiempoRestanteParaEsperar}ms ********", tiempoRestanteParaEsperar);
                }
                else
                {
                    _logger.LogInformation($"******** Sin tiempo de espera entre ciclos ********");
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"******** Solicitud de Cancelación ********");
            }
            finally
            {
                _logger.LogInformation("******** Fin ciclo ********");
                Environment.Exit(0);
            }
        }
    }
}