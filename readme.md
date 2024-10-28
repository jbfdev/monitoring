# JBF Monitoring Compliance

To comply with our monitoring requirements, all API must include the following endpoint for health checks. These ensure that the system health and readiness are monitored and reported correctly.

## Required endpoints

- **Health Check Endpoint** (`/health-check`):  
  Provides a detailed health status report of the application based on registered health checks.

- **Health Probe Endpoint** (`/health-probe`):  
  Offers a quick, simplified status check (e.g., "Healthy", "Unhealthy") suitable for readiness probes or load balancers.

## Implementation

To implement custom health checks, inherit the HealthCheckBase.

```csharp
public class SampleHealthCheck : HealthCheckBase
{
    protected override Task<JBF.Core.Monitoring.HealthChecks.HealthCheckResult> CheckHealth(HealthCheckContext context, CancellationToken ct = default)
    {
        //Implement custom health check logic
    }
}
```

### AspNet Core
Add the following code to your AspNet Core app to register and configure the monitoring services:

```csharp
using JBF.Monitoring;

var builder = WebApplication.CreateBuilder(args);

// Add and configures the required services for monitoring
builder.AddMonitoring(configureHealthChecks: health =>
{
    health.AddCheck<SampleHealth>("Sample"); // Example health check
});

var app = builder.Build();

// Add required endpoints for health checks
app.UseMonitoring();

app.Run();
```

### Azure Functions

Add the following code to your Azure Funtions app to register and configure the monitoring services:

```csharp
using JBF.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddMonitoring(
            configuration: context.Configuration,
            environment: context.HostingEnvironment,
            configureHealthChecks: health =>
            {
                health.AddCheck<SampleHealth>("Sample"); // Example health check
            });
    })
    .Build();

host.Run();
```

Additionally, add the following Http triggered functions:

```csharp
using JBF.Monitoring.HealthChecks;
using JBF.Monitoring.Manifests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class HealthCheckFunctions(HealthCheckService healthCheckService, Manifest manifest)
{
    [Function(nameof(HealthCheck))]
    public async Task<IActionResult> HealthCheck(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health-check")] HttpRequest req, CancellationToken ct)
    {
        var report = await healthCheckService.CheckHealthAsync(ct);
        var mappedReport = HealthReportMapper.CreateFrom(manifest, report);

        return new OkObjectResult(mappedReport);
    }

    [Function(nameof(HealthProbe))]
    public async Task<IActionResult> HealthProbe(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health-probe")] HttpRequest req, CancellationToken ct)
    {
        var report = await healthCheckService.CheckHealthAsync((_) => false, ct);
        return new OkObjectResult(report.Status.ToString());
    }
}
```

## Local development

### OpenTelemetry (OTLP) Exporter
By adding the following configuration to your App, you can enable the OpenTelemetry (OTLP) Exporter for sending telemetry data to a specified endpoint:

```plaintext
OTEL_EXPORTER_OTLP_ENDPOINT=<Your_OTLP_Endpoint_URL>
```

### Azure Monitor (Application Insights)
To enable Azure Monitor, add the following configuration setting to your Azure Function App:

```plaintext
APPLICATIONINSIGHTS_CONNECTION_STRING=<Your_Application_Insights_Connection_String>
```