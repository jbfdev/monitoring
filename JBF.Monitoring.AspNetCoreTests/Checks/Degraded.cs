using JBF.Monitoring.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JBF.Monitoring.AspNetCoreTests.Checks;

public class Degraded : HealthCheckBase
{
    protected override async Task<HealthCheck> CheckHealth(HealthCheckContext context, CancellationToken ct = default)
    {
        return await Task.FromResult(HealthCheck.Degraded("Ich bin eine Teilweise erfolgreicher Gesundheitscheck"));
    }
}