using JBF.Core.Monitoring.Manifests;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JBF.Core.Monitoring.HealthChecks;

public static class HealthReportMapper
{
    public static HealthCheckReport CreateFrom(Manifest manifest, HealthReport report)
    {
        HealthCheckReport healthCheckResponse = new()
        {
            Name = manifest.Name,
            Environment = manifest.Environment,
            Status = report.Status.ToString(),
            SystemStartup = manifest.SystemStartup,
        };

        foreach (var item in report.Entries)
        {
            var updated = (DateTimeOffset)item.Value.Data[nameof(HealthCheckResult.Updated)];
            var issues = (string[])item.Value.Data[nameof(HealthCheckResult.Issues)];
            var statusMessage = (string)item.Value.Data[nameof(HealthCheckResult.StatusMessage)];

            HealthCheckResult healthCheckEntry = new(item.Value.Status)
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

    public static Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult CreateFrom(HealthCheckResult healthCheckEntry)
    {
        var data = new Dictionary<string, object>
        {
            { nameof(HealthCheckResult.Updated), healthCheckEntry.Updated },
            { nameof(HealthCheckResult.Issues), healthCheckEntry.Issues },
            { nameof(HealthCheckResult.StatusMessage), healthCheckEntry.StatusMessage ?? "" }
        };

        return new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult(
            status: healthCheckEntry.Status,
            description: healthCheckEntry.Description,
            data: data);
    }
}
