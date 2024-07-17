using Receptores.DB;
using Receptores.Interfaces;
using Receptores.Interfaces.IRunners;
using Receptores.Services;

namespace Receptores
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            using IServiceScope ejecutar = host.Services.CreateScope();
            Worker worker = ejecutar.ServiceProvider.GetRequiredService<Worker>();
            await worker.ExecuteAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             .ConfigureServices((hostContext, services) =>
             {
                 IConfiguration configuration = hostContext.Configuration;
                 services.AddScoped<IDatabaseQueries, DatabaseQueries>();
                 services.AddScoped<IRunner, Runner>();
                 services.AddScoped<IProcesarCuentas, ProcesarCuentas>();
                 services.AddScoped<Worker>();
                 services.AddHttpClient("Radicacion", options =>
                 {
                     options.BaseAddress = new Uri($"{configuration.GetSection("Servicios:AGE").Value}{configuration.GetSection("Endpoints:ServicioAGE-GestionarCorreo").Value}");
                     options.Timeout = TimeSpan.FromMinutes(10);
                 });
                 services.AddLogging(builder =>
                 {
                     builder.AddSimpleConsole(options =>
                     {
                         options.SingleLine = true;
                         options.IncludeScopes = true;
                         options.TimestampFormat = "yyyy/mm/dd HH:mm:ss ";
                     });
                 });
             });
    }
}