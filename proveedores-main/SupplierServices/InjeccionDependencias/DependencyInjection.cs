using Application.Common.Behaviours;
using Application.Mappers;
using Application.UsesCases.Interactors;
using Application.Validators;
using Domain.Interfaces;
using Domain.Responses;
using FluentValidation;
using Infrastructure.DataBase;
using Infrastructure.Helpers;
using Infrastructure.LogAzure;
using Infrastructure.Repository;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace SupplierServices.Web.InjeccionDependencias
{
    /// <summary>
    /// Servicio de injección de dependencias, para evitar injectar en el program y este quede demasiado grande.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Servicio de injección de dependencias.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddServicesCustom(this IServiceCollection services, IConfiguration configuration)
        {
            //DbContext
            services.AddDbContext<ProvidersContext>(opt => opt.UseSqlServer("name=DefaultConnection"));

            //Include Helpers
            services.AddScoped<IContextHelper, ContextHelper>();
            services.AddScoped<IHelper, Helper>();

            //Include HttpContextAccessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //LogAzure
            services.AddScoped<ILogAzure, LogAzure>();

            //Responses
            services.AddScoped<IGeneralResponse, GeneralResponse>();

            //Repositories
            services.AddScoped<IProvidersRepository, ProvidersRepository>();

            //MediatR
            services.AddMediatR(conf => conf.RegisterServicesFromAssemblies(typeof(PaginateProvidersInteractor).Assembly));

            //Validators
            services.AddValidatorsFromAssembly(typeof(ActivateProviderValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(PaginateProvidersValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(LinkUsersProvidersValidator).Assembly);
            services.AddValidatorsFromAssembly(typeof(ProvidersStatusValidator).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviours<,>));

            //Servicio de mapper
            services.AddMapster();
            var config = TypeAdapterConfig.GlobalSettings;
            config.Apply(new ProfilesMapper());
            var mapper = new Mapper(config);
            services.AddSingleton<IMapper>(mapper);

            return services;
        }
    }
}
