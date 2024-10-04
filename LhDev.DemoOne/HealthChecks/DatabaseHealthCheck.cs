using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LhDev.DemoOne.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    /// <summary>
    /// For the purposes of this demo, the result is always healthy as it uses SQLite local file for data storage. For a
    /// production environment, use this to check the status of an MS SQL Server instance or similar.
    /// </summary>
    /// <param name="context">Context of health check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(HealthCheckResult.Healthy($"Database is healthy."));
}