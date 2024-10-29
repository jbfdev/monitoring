using JBF.Monitoring.Manifests;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JBF.Monitoring.HealthChecks;

public static class HealthReportMapper
{
    public static HealthCheckReport CreateFrom(Manifest manifest, HealthReport report)
    {
        HealthCheckReport healthCheckResponse = new()
        {
            Name = manifest.Name,
            Environment = manifest.Environment,
            Status = report.Status,
            SystemStartup = manifest.SystemStartup,
        };

        foreach (var item in report.Entries)
        {
            var updated = (DateTimeOffset)item.Value.Data[nameof(HealthCheck.Updated)];
            var issues = (string[])item.Value.Data[nameof(HealthCheck.Issues)];
            var statusMessage = (string)item.Value.Data[nameof(HealthCheck.StatusMessage)];

            HealthCheck healthCheckEntry = new(item.Value.Status)
            {
                Description = item.Key,
                Status = item.Value.Status,
                StatusMessage = statusMessage,
                Updated = updated,
                Issues = issues
            };

            healthCheckResponse.Issues.AddRange(issues.Select(i => $"{healthCheckEntry.Description}: {i}"));
            healthCheckResponse.Checks.Add(healthCheckEntry);
        }

        return healthCheckResponse;
    }

    public static HealthCheckResult CreateFrom(HealthCheck healthCheckEntry)
    {
        var data = new Dictionary<string, object>
        {
            { nameof(HealthCheck.Updated), healthCheckEntry.Updated },
            { nameof(HealthCheck.Issues), healthCheckEntry.Issues },
            { nameof(HealthCheck.StatusMessage), healthCheckEntry.StatusMessage ?? "" }
        };

        return new HealthCheckResult(
            status: healthCheckEntry.Status,
            description: healthCheckEntry.Description,
            data: data);
    }
}
