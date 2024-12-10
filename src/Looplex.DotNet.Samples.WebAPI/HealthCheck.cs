using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Looplex.DotNet.Samples.WebAPI;

public class HealthCheck() : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
    }
}
