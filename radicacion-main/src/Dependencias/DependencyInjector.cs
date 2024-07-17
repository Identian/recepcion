using AutoMapper;
using CapaDominio.Auth;
using CapaDominio.Interfaces.IAuth;
using CapaDominio.Interfaces.IDB;
using CapaDominio.Interfaces.IHelpers;
using CapaDominio.Interfaces.IReceptores;
using CapaDominio.Interfaces.IRepository;
using CapaDominio.Interfaces.IServices;
using CapaDominio.Interfaces.IServices.IServicesDocuments;
using CapaDominio.Interfaces.LogsAzure;
using CapaDominio.Interfaces.ServiceOutlook;
using CapaDominio.Receptor;
using CapaDominio.RequestReceptor;
using CapaDominio.Response;
using CasosDeUso.Interactors.GestionarReceptorInteractor;
using CasosDeUso.ValidatorDto;
using CasosDeUso.Validators;
using DTO.Automapper;
using FluentValidation;
using Infraestructura.DB;
using Infraestructura.Helpers;
using Infraestructura.Logs;
using Infraestructura.Repository;
using Infraestructura.Services.AuthServices;
using Infraestructura.Services.ConsultaApi;
using Infraestructura.Services.ConsultarReceptoresApi;
using Infraestructura.Services.DocumentosElectronico;
using Infraestructura.Services.EnviarDocumentos;
using Infraestructura.Services.GestionEmail;
using Infraestructura.Services.Login;
using Infraestructura.Services.RadicacionDocumentos;
using Infraestructura.Services.RecepcionDeMensajes;
using Infraestructura.Services.RecepciónDocumentosZip;
using Infraestructura.Services.ServiceOutlook;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dependencias
{
    public static class DependencyInjector
    {
        public static IServiceCollection AddServicesController(this IServiceCollection services, IConfiguration configuration)
        {
            //Domain
            services.AddScoped<IInfoReceptor, InfoReceptor>();
            services.AddScoped<ICuentaCorreo, CuentaCorreo>();
            services.AddScoped<IReceptor, Receptor>();
            services.AddScoped<IReceptorBase, ReceptorBase>();
            services.AddScoped<IRespuesta, Respuesta>();
            services.AddScoped<IEmailManagement, EmailManagement>();
            services.AddScoped<IEmailConectar, EmailConectar>();
            services.AddScoped<IDataBase, DataBase>();
            services.AddScoped<IRespuesta, Respuesta>();
            services.AddScoped<IRespuestaApiConsultar, RespuestaApiConsultar>();
            services.AddScoped<IRespuestaApi, RespuestaApi>();
            services.AddScoped<IObjectConversion<CuentaCorreo>, ObjectConversion<CuentaCorreo>>();
            services.AddScoped<IObjectConversion<State>, ObjectConversion<State>>();
            services.AddScoped<IListadoCorreo, ListadoCorreo>();
            services.AddScoped<IOutlook, Outlook>();

            //Helpers
            services.AddScoped<IUtils, Utils>();
            services.AddScoped<IDataBaseHelper, DataBaseHelper>();
            //procesos
            services.AddScoped<IProcesoRecepcionMensajes, ProcesoRecepcionMensajes>();
            services.AddScoped<IProcesoGestionEmail, ProcesoGestionEmail>();
            services.AddScoped<IProcesoRecepcionDocumentoZip, ProcesoRecepcionDocumentoZip>();
            //Documetos
            services.AddScoped<IDocumentoElectronico, DocumentoElectronico>();

            //Repositorios
            services.AddScoped<IGestionReceptoresRepository, GestionReceptoresRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IRegistrarEmailRepository, RegistrarEmailRepository>();
            services.AddScoped<IEmailInactivarRepository, EmailInactivarRepository>();
            services.AddScoped<IConsultarEmailRepository, ConsultarEmailRepository>();
            services.AddScoped<ICheckSilentAuthorizationRepository, CheckSilentAuthorizationRepository>();
            services.AddScoped<ISaveTenantRepository, SaveTenantRepository>();

            //services
            services.AddScoped<IConsultarReceptorDesdeApi, ConsultarReceptorDesdeApi>();
            services.AddScoped<IConsultarCuentaCorreoReceptorDesdeApi, ConsultarCuentaCorreoReceptorDesdeApi>();
            services.AddScoped<IProcesoAuthentication, ProcesoAuthentication>();
            services.AddScoped<IRadicacion, Radicacion>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IEnviarXmlReceptorProcessor, EnviarXmlReceptorProcessor>();
            services.AddScoped<IEnviarXml, EnviarXml>();
            services.AddScoped<IEnviarRepresentacionGraficaProcesor, EnviarRepresentacionGraficaProcesor>();
            services.AddScoped<IEnviarRepresentacionGrafica, EnviarRepresentacionGrafica>();
            services.AddScoped<IEnviarANexoProcessor, EnviarANexoProcessor>();
            services.AddScoped<IEnviarAnexo, EnviarAnexo>();
            services.AddScoped<IMultiservices, Multiservices>();

            //HttpClient
            services.AddHttpClient("Login", options =>
            {
                options.BaseAddress = new Uri($"{configuration.GetSection("urlBaseLogin").Value}{configuration.GetSection("urlServiceLogin").Value}");
            });
            services.AddHttpClient("EnviarXML", options =>
            {
                options.BaseAddress = new Uri($"{configuration.GetSection("urlBaseService").Value}{configuration.GetSection("urlServiceAPIEnviarXMLReceptor").Value}");
            });

            services.AddHttpClient("EnviarRepGrafica", options =>
            {
                options.BaseAddress = new Uri($"{configuration.GetSection("urlBaseService").Value}{configuration.GetSection("urlServiceAPIEnviarRepGrafica").Value}");
            });
            services.AddHttpClient("EnviarAnexo", options =>
            {
                options.BaseAddress = new Uri($"{configuration.GetSection("urlBaseService").Value}{configuration.GetSection("urlServiceAPIEnviarAnexo").Value}");
            });

            //Auth
            services.AddScoped<ILoginSoap, LoginSoap>();
            services.AddScoped<ILoginProcesor, LoginProcesor>();

            //Log
            services.AddScoped<ILogAzure, LogAzure>();
            //MediatR
            services.AddMediatR(ser =>
            {
                ser.RegisterServicesFromAssemblies(typeof(InfoReceptorInteractor).Assembly);
            });

            //mapper
            MapperConfiguration mapperConfig = new(cf =>
            {
                cf.AddProfile(new MapperProfiles());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            //FluentValidation
            services.AddValidatorsFromAssembly(typeof(EmailInputValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(ConsultaEmailValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(InactivarCorreoValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(SaveEmailValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
