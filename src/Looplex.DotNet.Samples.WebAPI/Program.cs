using System.Text.Json;
using Looplex.DotNet.Core.Common.Logging;
using Looplex.DotNet.Core.Common.Middlewares;
using Looplex.DotNet.Core.Common.Utils;
using Looplex.DotNet.Core.Infra.IoC;
using Looplex.DotNet.Core.Infra.Profiles;
using Looplex.DotNet.Core.Middlewares;
using Looplex.DotNet.Core.WebAPI.ExtensionMethods;
using Looplex.DotNet.Core.WebAPI.Factories;
using Looplex.DotNet.Core.WebAPI.Middlewares;
using Looplex.DotNet.Core.WebAPI.Routes;
using Looplex.DotNet.Middlewares.Clients.ExtensionMethods;
using Looplex.DotNet.Middlewares.OAuth2.DTOs;
using Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;
using Looplex.DotNet.Middlewares.OAuth2.Services;
using Looplex.DotNet.Middlewares.OAuth2.Storages.Default.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Looplex.DotNet.Samples.Academic.Infra.Data.Commands;
using Looplex.DotNet.Samples.Academic.Infra.IoC;
using Looplex.DotNet.Samples.Academic.Infra.Profiles;
using Looplex.DotNet.Samples.WebAPI.Extensions;
using Looplex.DotNet.Samples.WebAPI.Factories;
using Looplex.DotNet.Samples.WebAPI.Routes;
using Looplex.DotNet.Samples.WebAPI.Routes.Academic;
using Looplex.DotNet.Services.ScimV2.InMemory.ExtensionMethods;
using Looplex.DotNet.Services.ScimV2.InMemory.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
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

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(config =>
            {
                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                config.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });

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
            app.UseClientRoutes(DefaultScimV2RouteOptions.CreateFor<ClientService>());
            app.UseUserRoutes(DefaultScimV2RouteOptions.CreateFor<UserService>());
            app.UseGroupRoutes(DefaultScimV2RouteOptions.CreateFor<GroupService>());
            app.UseStudentRoutes();
            // app.UseSchoolSubjectRoutes();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();            

            app.Run();
        }
        
        private static void RegisterServices(IServiceCollection services)
        {
            AcademicDependencyContainer.RegisterServices(services);

            RegisterMediatR(services);

            RegisterAutoMapperProfiles(services);

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

        private static void RegisterAutoMapperProfiles(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AcademicProfile));
            
            services.AddCoreAutoMapper();
            services.AddClientsAutoMapper();
            services.AddScimV2AutoMapper();
            services.AddOAuth2AutoMapper();
            services.AddClientsInMemoryAutoMapper();
            services.AddScimV2InMemoryAutoMapper();
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
            builder.Services.AddControllers(options =>
            {
                options.CacheProfiles.Add(Constants.Cache_Default30s,
                    new CacheProfile()
                    {
                        Duration = 30
                    });
            });
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
    }
}