using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LhDev.DemoOne.HealthChecks;

public class RandomTestHealthCheck : IHealthCheck
{
    private readonly Random _random = new();

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var responseTime = _random.Next(1, 300);
        return responseTime switch
        {
            < 100 => Task.FromResult(HealthCheckResult.Healthy($"Healthy result ({responseTime})")),
            < 200 => Task.FromResult(HealthCheckResult.Degraded($"Degraded result ({responseTime})")),
            _ => Task.FromResult(HealthCheckResult.Unhealthy($"Unhealthy result ({responseTime})"))
        };
    }
}