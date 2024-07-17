using APIGateway.Extensions;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// configuración JSON
builder.Configuration.AddOcelotConfigFiles(Environment.GetEnvironmentVariable("NAMESPACE") ?? "", new[]
{
    "Base", //First Item for base
    "AgeEmailManage",
    "Metadata",
    "Providers",
    "Eventos"
});

builder.Services.AddOcelot();

builder.Services.AddEndpointsApiExplorer();
string _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = $"v{Assembly.GetExecutingAssembly().GetName().Version}",
        Title = "API Gateway Recepcion - " + (string.IsNullOrEmpty(_environment) ? "Development" : _environment) + " - Namespace: " + Environment.GetEnvironmentVariable("NAMESPACE"),
        Description = "API Gateway Recepcion ",
    });
});

WebApplication app = builder.Build();

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
    options.DocumentTitle = "API Gateway Recepcion";
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.UseOcelot().Wait();

app.Run();
