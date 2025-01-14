using Casbin;
using Looplex.DotNet.Middlewares.ApiKeys.ExtensionMethods;
using Looplex.DotNet.Middlewares.OAuth2.ExtensionMethods;
using Looplex.DotNet.Middlewares.ScimV2.ExtensionMethods;
using Looplex.DotNet.Services.ApiKeys.InMemory.ExtensionMethods;
using Looplex.DotNet.Services.Redis.ExtensionMethods;
using Looplex.DotNet.Services.ScimV2.InMemory.ExtensionMethods;
using Looplex.DotNet.Services.SqlDatabases.ExtensionMethods;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using RestSharp;

namespace Looplex.DotNet.WebApi;

public abstract class BaseProgram
{
    protected IConfiguration Configuration { get; private set; } = null!;
    
    public virtual void Run(string[] args)
    {
        Configuration = new ConfigurationBuilder()
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

        RegisterServices(builder.Services);

        //ConfigureLogging(builder, configuration);
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

        var schemaIdApiKey = Configuration["JsonSchemaIdForClientCredential"]!;
        var schemaIdUser = Configuration["JsonSchemaIdForUser"]!;
        var schemaIdGroup = Configuration["JsonSchemaIdForGroup"]!;

        app.UseTokenRoute(["AuthorizationService.CreateAccessToken"]);
        app.UseSchemaRoute([]);
        app.UseResourceTypeRoute([]);
        app.UseServiceProviderConfigurationRoute([]);
        app.UseBulkRoute([]);
        app.UseApiKeyRoutesAsync(schemaIdApiKey).GetAwaiter().GetResult();
        app.UseUserRoutesAsync(schemaIdUser).GetAwaiter().GetResult();
        app.UseGroupRoutesAsync(schemaIdGroup).GetAwaiter().GetResult();

        UseRoutes(app);
            
        app.UseHttpsRedirection();

        app.Run();
    }
        
    /// <summary>
    /// Should register routes for the api
    /// <example>app.UseStudentRoutesAsync(schemaIdStudent, CancellationToken.None).GetAwaiter().GetResult();</example>
    /// </summary>
    protected abstract void UseRoutes(IEndpointRouteBuilder app);

    private void RegisterServices(IServiceCollection services)
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
            
        var redisConnectionString = Configuration["RedisConnectionString"]!;
        AddSecretsService(services);
        AddServices(services);
        AddFactories(services);
            
        services.AddRedisServices(redisConnectionString);
        services.AddSqlDatabaseServices();

        services.AddApiKeyServices();
        services.AddScimV2Services();
        services.AddOAuth2Services(InitRbacEnforcer());
        services.AddApiKeyInMemoryServices();
        services.AddScimV2InMemoryServices();
    }

    /// <summary>
    /// Should register a implementation for ISecretService in the dependency container
    /// <example>services.AddSingleton<ISecretsService, InMemorySecretsService>();</example>
    /// </summary>
    protected abstract void AddSecretsService(IServiceCollection services);

    /// <summary>
    /// Should register required services for the application such as
    /// ICrudService implementations and MediatR registrations 
    /// <example>services.AddAcademicServices();</example>
    /// </summary>
    protected abstract void AddServices(IServiceCollection services);
        
    /// <summary>
    /// Should register implementations for required factories such as IContextFactory
    /// <example>services.AddTransient<IContextFactory, ContextFactory>();</example>
    /// </summary>
    protected abstract void AddFactories(IServiceCollection services);
        
    private static void ConfigureResponseCache(WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCaching();
    }

    protected abstract IEnforcer InitRbacEnforcer();

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

    protected virtual IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handle transient errors (5xx, 408, etc.)
            .WaitAndRetryAsync(3,
                retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))); // Retry 3 times with exponential backoff
    }
}