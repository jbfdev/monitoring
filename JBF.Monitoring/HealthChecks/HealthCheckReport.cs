using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json.Serialization;

namespace JBF.Monitoring.HealthChecks;

public record HealthCheckReport
{
    public required string Name { get; init; }
    public required HealthStatus Status { get; init; }
    public List<string> Issues { get; } = [];
    public required DateTimeOffset SystemStartup { get; init; }
    public required string Environment { get; init; }
    public List<HealthCheck> Checks { get; } = [];
}

public record HealthCheck
{
    public string? Description { get; init; }
    public HealthStatus Status { get; init; }
    public DateTimeOffset Updated { get; init; }
    public string? StatusMessage { get; init; }
    public string[] Issues { get; init; } = [];
    public IDictionary<string, object> Data { get; private set; }

    public HealthCheck(HealthStatus status, string? statusMessage = null, List<string>? issues = null, IDictionary<string, object>? data = null)
    {
        Status = status;
        StatusMessage = statusMessage;
        Updated = DateTimeOffset.UtcNow;
        Issues = issues?.ToArray() ?? [];
        Data = data ?? new Dictionary<string, object>();
    }

    public static HealthCheck Healthy(string? statusMessage = null, IDictionary<string, object>? data = null)
    {
        return new HealthCheck(status: HealthStatus.Healthy, statusMessage: statusMessage, data: data);
    }

    public static HealthCheck Degraded(string? statusMessage = null, List<string>? issues = null, IDictionary<string, object>? data = null)
    {
        return new HealthCheck(HealthStatus.Degraded, statusMessage, issues, data);
    }

    public static HealthCheck Unhealthy(string? statusMessage = null, List<string>? issues = null, IDictionary<string, object>? data = null)
    {
        return new HealthCheck(HealthStatus.Unhealthy, statusMessage, issues, data);
    }
}