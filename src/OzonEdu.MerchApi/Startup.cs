using FluentValidation;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OzonEdu.MerchApi.Domain.AggregationModels.MerchPackAggregate;
using OzonEdu.MerchApi.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchApi.Domain.Contracts;
using OzonEdu.MerchApi.Domain.Services;
using OzonEdu.MerchApi.GrpcServices;
using OzonEdu.MerchApi.Infrastructure.Configuration;
using OzonEdu.MerchApi.Infrastructure.Repositories.Implementation;
using OzonEdu.MerchApi.Infrastructure.Repositories.Implementation.MerchPackPostgreRepository;
using OzonEdu.MerchApi.Infrastructure.Repositories.Implementation.MerchRequestPostgreRepository;
using OzonEdu.MerchApi.Infrastructure.Repositories.Implementation.Mock;
using OzonEdu.MerchApi.Infrastructure.Repositories.Infrastructure;
using OzonEdu.MerchApi.Infrastructure.Repositories.Infrastructure.Interfaces;
using OzonEdu.MerchApi.Infrastructure.Services.Implementation;
using OzonEdu.MerchApi.Services.Handlers.MerchRequestAggregate;
using OzonEdu.MerchApi.Services.PipelineBehaviors.UnitOfWorkBehavior;

namespace OzonEdu.MerchApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            AddDatabaseComponents(services);
            AddMockServices(services);
            AddPostgreRepositories(services);
            AddMediator(services);
        }
        

        private static void AddMediator(IServiceCollection services)
        {
            services.AddMediatR(typeof(IssueMerchCommandHandler));
            services.AddValidatorsFromAssembly(typeof(IssueMerchCommandHandler).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
        }
        
        private void AddDatabaseComponents(IServiceCollection services)
        {
            services.Configure<DatabaseConnectionOptions>(Configuration.GetSection(nameof(DatabaseConnectionOptions)));
            services.AddScoped<IDbConnectionFactory<NpgsqlConnection>, NpgsqlConnectionFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IChangeTracker, ChangeTracker>();
        }
        
        private static void AddPostgreRepositories(IServiceCollection services)
        {
            services.AddScoped<IMerchPackRepository, MerchPackPostgreRepository>();
            services.AddScoped<IMerchRequestRepository, MerchRequestPostgreRepository>();
        }
        private static void AddMockRepositories(IServiceCollection services)
        {
            services.AddSingleton<IMerchPackRepository, MerchPackMockRepository>();
            services.AddSingleton<IMerchRequestRepository, MerchRequestMockRepository>();
        }
        private static void AddMockServices(IServiceCollection services)
        {
            services.AddScoped<IEmailService, EmailMockService>();
            services.AddScoped<IStockApiService, StockApiMockService>();
        }
        
        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<MerchandiseGrpcServices>();
                endpoints.MapControllers();
            });
        }
    }
}