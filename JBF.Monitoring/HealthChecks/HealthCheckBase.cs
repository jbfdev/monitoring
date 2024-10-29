using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JBF.Monitoring.HealthChecks;

public abstract class HealthCheckBase : IHealthCheck
{
    protected abstract Task<HealthCheck> CheckHealth(HealthCheckContext context, CancellationToken ct = default);

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var healthCheck = await CheckHealth(context, cancellationToken);
        var report = healthCheck with
        {
            Updated = TimeProvider.System.GetUtcNow(),
            Description = context.Registration.Name
        };

        return HealthReportMapper.CreateFrom(report);
    }
}