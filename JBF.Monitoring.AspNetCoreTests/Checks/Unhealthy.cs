using JBF.Monitoring.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JBF.Monitoring.AspNetCoreTests.Checks;

public class Unhealthy : HealthCheckBase
{
    protected override async Task<HealthCheck> CheckHealth(HealthCheckContext context, CancellationToken ct = default)
    {
        return await Task.FromResult(HealthCheck.Unhealthy("Mir geht es schlecht", ["Bauchschmerzen", "Fieber"]));
    }
}