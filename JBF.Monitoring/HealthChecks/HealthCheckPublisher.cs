using Grpc.Core;
using JBF.Core.Monitoring.Manifests;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace JBF.Core.Monitoring.HealthChecks;

public class HealthCheckPublisher(Manifest manifest, ILogger<HealthCheckPublisher> logger) : IHealthCheckPublisher
{
    private static readonly EventId _healthCheckEventId = new(4317);

    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        var mappedReport = HealthReportMapper.CreateFrom(manifest, report);

        foreach (var entry in mappedReport.Checks)
        {
            logger.LogInformation(_healthCheckEventId, "Health check: {ServiceName} {ServiceStatus} {ServiceUpSince} {CheckDescription} {CheckStatus}, {CheckStatusMessage}, {CheckIssues}", 
                mappedReport.Name, mappedReport.Status, mappedReport.SystemStartup, entry.Description, entry.Status, entry.StatusMessage, string.Join(';', entry.Issues));
        }

        return Task.CompletedTask;
    }
}
