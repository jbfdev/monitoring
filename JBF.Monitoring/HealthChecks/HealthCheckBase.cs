using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JBF.Core.Monitoring.HealthChecks;

public abstract class HealthCheckBase : IHealthCheck
{
    protected abstract Task<HealthCheckResult> CheckHealth(HealthCheckContext context, CancellationToken ct = default);

    public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var healthCheckEntry = await CheckHealth(context, cancellationToken);
        healthCheckEntry.Updated = TimeProvider.System.GetUtcNow();
        healthCheckEntry.Description = context.Registration.Name;

        return HealthReportMapper.CreateFrom(healthCheckEntry);
    }
}