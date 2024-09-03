using Looplex.DotNet.Core.Application.Abstractions.DataAccess;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Looplex.DotNet.Samples.WebAPI;

public class HealthCheck(IDatabaseContext databaseContext) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;
        var exceptions = new List<Exception>();
        var services = new Dictionary<string, object>
        {
            { "self", true },
            { "db", true }
        };

        if (!CheckDatabase(out var exception))
        {
            isHealthy = false;
            exceptions.Add(exception!);
            services["db"] = false;
        }
        
        if (isHealthy)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("A healthy result.", services));
        }
        return Task.FromResult(
            new HealthCheckResult(
                context.Registration.FailureStatus, 
                "An unhealthy result.",
                new AggregateException(exceptions),
                services));
    }

    private bool CheckDatabase(out Exception? exception)
    {
        exception = null;
        using var connection = databaseContext.CreateConnection();
        try
        {
            connection.Open();
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
        return true;
    }
}
