using CasosDeUso.Exceptions;
using CasosDeUso.Exceptions.Base;
using CasosDeUso.Exceptions.Filter;
using CasosDeUso.Exceptions.Presentadores;
using Dependencias;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => options.Filters.Add(new ApiFilterExceptions(new Dictionary<Type, IExeptionHandler>
{
    { typeof(GeneralException),     new GeneralExceptionHandler() },
    { typeof(ValidationException),  new ValidationExceptionHandler() }
})));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    ConfigurationManager configuration = builder.Configuration;

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = $"v{Assembly.GetExecutingAssembly().GetName().Version}",
        Title = $"Radicación - {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}",
        Description = $"Gestión de Emails y Radicación de Documentos - {configuration["Version:Comment"]}"
    });
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

ConfigurationManager config = builder.Configuration;

//Rutas Paravalidar el estado de las conexiones de los servicios y base de datos.
builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(config.GetSection("urlBaseService").Value ?? ""), HttpMethod.Post, name: "AGE", HealthStatus.Unhealthy, new string[] { "Servicio AGE" })
    .AddUrlGroup(new Uri(config.GetSection("urlBaseLogin").Value ?? ""), HttpMethod.Post, name: "TestLogin", HealthStatus.Unhealthy, new string[] { "LoginSoap" })
    .AddSqlServer(config.GetSection("ConnectionString").Value ?? "", null, name: "Database", failureStatus: HealthStatus.Unhealthy, new string[] { "DataBase" });

//Capa de injección de dependencias ubicación Capa Dependencias
builder.Services.AddServicesController(builder.Configuration);


WebApplication app = builder.Build();


app.UseHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false,

});

// Configure the HTTP request pipeline.
app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
