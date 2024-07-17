using Microsoft.OpenApi.Models;
using System.Reflection;
using Application.Common.Filters;
using Application.Common.Interfaces;
using Domain.Exceptions;
using Application.Common.Presenters;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Infrastructure.DataBase;
using SupplierServices.Web.InjeccionDependencias;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options => options.Filters.Add(
    new ApiFilterExceptionAttribute(new Dictionary<Type, IExceptionHandler> {
        { typeof(GeneralException), new GeneralExceptionHandler() },
        { typeof(ValidationException), new ValidationExceptionHandler() }
    }) 
)).AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    ConfigurationManager configuration = builder.Configuration;
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = $"{Assembly.GetExecutingAssembly().GetName().Version}",
        Title = $"MicroServicio de proveedores - { Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") }",
        Description = "Proveedores."
    });
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

//Servicio de injección de dependencias perzonalizado 
builder.Services.AddServicesCustom(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {

        string issuer = builder.Configuration.GetSection("Token:Issuer").Value ?? "";
        var audience = builder.Configuration.GetSection("Token:Audience").Value ?? "";
        var key = builder.Configuration.GetSection("Token:Key").Value ?? "";

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateActor = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

    });

var config = builder.Configuration;

builder.Services.AddHealthChecks()
    .AddSqlServer(config.GetConnectionString("DefaultConnection") ?? "", "SELECT 1;", null, null, HealthStatus.Unhealthy, ["DataBase"])
    .AddDbContextCheck<ProvidersContext>();
    

var app = builder.Build();

app.UseHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false,

});




// Configure the HTTP request pipeline.
app.UseSwagger(option =>
{
    option.SerializeAsV2 = true;
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
    options.DocumentTitle = "Api Recepción - Proveedores";
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
