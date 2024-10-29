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
            SystemStartup = manifest.SystemStartup
        };

        foreach (var item in report.Entries)
        {
            try
            {
                if (!item.Value.Data.TryGetValue(nameof(HealthCheck.Updated), out var updatedObj)
                    && updatedObj is not DateTimeOffset)
                    continue;
                var updated = (DateTimeOffset)updatedObj;
            
                if (!item.Value.Data.TryGetValue(nameof(HealthCheck.Issues), out var issuesObj)
                    && issuesObj is not string[])
                    continue;
                var issues = (string[]?)issuesObj ?? [];
            
                if (!item.Value.Data.TryGetValue(nameof(HealthCheck.Updated), out var statusMessageObj)
                    && statusMessageObj is not string)
                    continue;
                var statusMessage = (string)statusMessageObj;

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
            catch
            {
                //Continue silently on non-compliant HealthCheckEntry
            }
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
