using System.Net;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.WebAPI.ExtensionMethods;
using Looplex.DotNet.Middlewares.Clients.Domain.Entities.Clients;
using Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Groups;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Schemas;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Users;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Looplex.DotNet.Samples.Academic.Domain.Entities.Students;
using Looplex.DotNet.Samples.Academic.Infra.Data.Commands;
using Looplex.DotNet.Samples.Academic.Infra.IoC;
using Looplex.DotNet.Samples.WebAPI.Factories;
using Looplex.DotNet.Samples.WebAPI.Routes;
using Looplex.DotNet.Samples.WebAPI.Routes.Academic;
using Looplex.DotNet.Services.Clients.InMemory.ExtensionMethods;
using Looplex.DotNet.Services.Clients.InMemory.Services;
using Looplex.DotNet.Services.ScimV2.InMemory.ExtensionMethods;
using Looplex.DotNet.Services.ScimV2.InMemory.Services;
using MassTransit;
using Looplex.DotNet.Middlewares.Clients.ExtensionMethods;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Looplex.DotNet.Samples.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddEnvironmentVariables()
                .Build();

            var builder = WebApplication.CreateBuilder(args);
            
            RegisterServices(builder.Services);

            ConfigureLogging(builder, configuration);
            ConfigureResponseCache(builder);
            //ConfigureTelemetry(builder);
            builder.Services.AddMassTransit(config =>
            {
                // Configure the in-memory message broker
                config.UsingInMemory((context, cfg) => { });

                // config.AddConsumers(typeof(UserDeletedConsumer).Assembly);
            });

            var app = builder.Build();
            
            app.UseTokenRoute(["AuthorizationService.CreateAccessToken"]);
            app.UseClientRoutes(options: DefaultScimV2RouteOptions.CreateFor<ClientService>());
            app.UseUserRoutes(options: DefaultScimV2RouteOptions.CreateFor<UserService>());
            app.UseGroupRoutes(options: DefaultScimV2RouteOptions.CreateFor<GroupService>());
            app.UseStudentRoutes();
            
            AddSchemas();

            app.UseHttpsRedirection();

            app.Run();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            AcademicDependencyContainer.RegisterServices(services);

            RegisterMediatR(services);
            
            services.AddCoreServices();
            services.AddClientsServices();
            services.AddScimV2Services();
            services.AddOAuth2Services();
            services.AddClientsInMemoryServices();
            services.AddScimV2InMemoryServices();

            services.AddTransient<IContextFactory, ContextFactory>();
        }

        private static void RegisterMediatR(IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateStudentCommandHandler).Assembly));
        }

        private static void ConfigureLogging(WebApplicationBuilder builder, IConfigurationRoot configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            builder.Logging.AddSerilog();
        }

        private static void ConfigureResponseCache(WebApplicationBuilder builder)
        {
            builder.Services.AddResponseCaching();
        }

        private static void ConfigureTelemetry(WebApplicationBuilder builder)
        {

            const string serviceName = "looplex-dotnet-sample";
            builder.Logging.AddOpenTelemetry(options =>
            {
                options
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName))
                    .AddConsoleExporter();
            });
            builder.Services.AddOpenTelemetry()
                  .ConfigureResource(resource => resource.AddService(serviceName))
                  .WithTracing(tracing => tracing
                      .AddAspNetCoreInstrumentation()
                      .AddConsoleExporter())
                  .WithMetrics(metrics => metrics
                      .AddAspNetCoreInstrumentation()
                      .AddConsoleExporter());
        }

        private static void AddSchemas()
        {
            Schema.Add<User>(File.ReadAllText("/schemas/User.1.0.schema.json"));
            Schema.Add<Group>(File.ReadAllText("/schemas/Group.1.0.schema.json"));
            Schema.Add<Client>(File.ReadAllText("/schemas/Client.1.0.schema.json"));
            Schema.Add<Student>(File.ReadAllText("/schemas/Student.1.0.schema.json"));
        }
    }
}