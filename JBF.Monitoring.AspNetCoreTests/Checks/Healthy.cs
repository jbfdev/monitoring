using JBF.Monitoring.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JBF.Monitoring.AspNetCoreTests.Checks;

public class Healthy : HealthCheckBase
{
    protected override async Task<HealthCheck> CheckHealth(HealthCheckContext context, CancellationToken ct = default)
    {
        return await Task.FromResult(HealthCheck.Healthy());
    }
}