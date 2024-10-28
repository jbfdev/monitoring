using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json.Serialization;

namespace JBF.Monitoring.HealthChecks;

public record HealthCheckReport
{
    public required string Name { get; set; }
    public required HealthStatus Status { get; set; }
    public List<string> Issues { get; set; } = [];
    public required DateTimeOffset SystemStartup { get; set; }
    public required string Environment { get; set; }
    public List<HealthCheckResult> Checks { get; set; } = [];
}

public record HealthCheckResult
{
    public string? Description { get; set; }
    public HealthStatus Status { get; set; }
    public DateTimeOffset Updated { get; set; }
    public string? StatusMessage { get; set; }
    public string[] Issues { get; set; } = [];
    public IDictionary<string, object> Data { get; set; }

    public HealthCheckResult(HealthStatus status, string? statusMessage = null, List<string>? issues = null, IDictionary<string, object>? data = null)
    {
        Status = status;
        StatusMessage = statusMessage;
        Issues = issues?.ToArray() ?? [];
        Data = data ?? new Dictionary<string, object>();
    }

    public static HealthCheckResult Healthy(string? statusMessage = null, IDictionary<string, object>? data = null)
    {
        return new HealthCheckResult(status: HealthStatus.Healthy, statusMessage: statusMessage, data: data);
    }

    public static HealthCheckResult Degraded(string? statusMessage = null, List<string>? issues = null, IDictionary<string, object>? data = null)
    {
        return new HealthCheckResult(HealthStatus.Degraded, statusMessage, issues, data);
    }

    public static HealthCheckResult Unhealthy(string? statusMessage = null, List<string>? issues = null, IDictionary<string, object>? data = null)
    {
        return new HealthCheckResult(HealthStatus.Unhealthy, statusMessage, issues, data);
    }
}