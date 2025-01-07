using System.Reflection;
using Casbin;
using Looplex.DotNet.Core.Application.Abstractions.Factories;
using Looplex.DotNet.Core.Application.Abstractions.Services;
using Looplex.DotNet.Middlewares.ApiKeys.ExtensionMethods;
using Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Looplex.DotNet.Samples.Academic.Infra.IoC;
using Looplex.DotNet.Samples.WebAPI.Factories;
using Looplex.DotNet.Samples.WebAPI.Routes.Academic;
using Looplex.DotNet.Samples.WebAPI.Services;
using Looplex.DotNet.Services.ScimV2.InMemory.ExtensionMethods;
using MassTransit;
using Looplex.DotNet.Services.ApiKeys.InMemory.ExtensionMethods;
using Looplex.DotNet.Services.Redis.ExtensionMethods;
using Looplex.DotNet.Services.SqlDatabases.ExtensionMethods;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using RestSharp;
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

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient("Default")
                .AddPolicyHandler(GetRetryPolicy());
            builder.Services.AddHealthChecks()
                .AddCheck<HealthCheck>("Default");
            builder.Services.AddRedisHealthChecks();
            builder.Services.AddSqlDatabaseHealthChecks();
            builder.Services.AddMemoryCache();

            RegisterServices(builder.Services, configuration);

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

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = JsonConvert.SerializeObject(new
                    {
                        status = report.Status.ToString(),
                        results = report.Entries.Select(e => new
                        {
                            key = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            data = e.Value.Data,
                            exception = e.Value.Exception?.Message // Include exception details
                        })
                    });
                    await context.Response.WriteAsync(result);
                }
            });

            var schemaIdApiKey = configuration["JsonSchemaIdForClientCredential"]!;
            var schemaIdUser = configuration["JsonSchemaIdForUser"]!;
            var schemaIdGroup = configuration["JsonSchemaIdForGroup"]!;
            var schemaIdStudent = configuration["JsonSchemaIdForStudent"]!;

            app.UseTokenRoute(["AuthorizationService.CreateAccessToken"]);
            app.UseSchemaRoute([]);
            app.UseResourceTypeRoute([]);
            app.UseServiceProviderConfigurationRoute([]);
            app.UseBulkRoute([]);
            app.UseApiKeyRoutesAsync(schemaIdApiKey).GetAwaiter().GetResult();
            app.UseUserRoutesAsync(schemaIdUser).GetAwaiter().GetResult();
            app.UseGroupRoutesAsync(schemaIdGroup).GetAwaiter().GetResult();
            app.UseStudentRoutesAsync(schemaIdStudent, CancellationToken.None).GetAwaiter().GetResult();

            app.UseHttpsRedirection();

            app.Run();
        }

        private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IRestClient>(_ =>
            {
                var options = new RestClientOptions
                {
                    ThrowOnAnyError = false
                };
                return new RestClient(options);
            });
            services.AddTransient(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Default");
                return new RestClient(httpClient, new RestClientOptions
                {
                    ConfigureMessageHandler = _ => new HttpClientHandler() // Optionally customize HttpClientHandler
                });
            });
            services.AddSingleton<ISecretsService, InMemorySecretsService>();
            var redisConnectionString = configuration["RedisConnectionString"]!;
            services.AddRedisServices(redisConnectionString);
            services.AddSqlDatabaseServices();

            services.AddAcademicServices();
            services.AddApiKeyServices();
            services.AddScimV2Services();
            services.AddOAuth2Services(InitRbacEnforcer());
            services.AddApiKeyInMemoryServices();
            services.AddScimV2InMemoryServices();

            services.AddTransient<IContextFactory, ContextFactory>();
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

        private static IEnforcer InitRbacEnforcer()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                                ?? throw new InvalidOperationException("Could not determine directory");
            var modelPath = Path.Combine(dir, "model.conf");
            var policyPath = Path.Combine(dir, "policy.csv");

            if (!File.Exists(modelPath) || !File.Exists(policyPath))
            {
                throw new FileNotFoundException("Required Casbin configuration files are missing");
            }

            return new Enforcer(modelPath, policyPath);
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

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // Handle transient errors (5xx, 408, etc.)
                .WaitAndRetryAsync(3,
                    retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // Retry 3 times with exponential backoff
        }
    }
}