﻿using Azure.Monitor.OpenTelemetry.AspNetCore;
using JBF.Core.Monitoring.HealthChecks;
using JBF.Core.Monitoring.Manifests;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Reflection;

namespace JBF.Core.Monitoring.Functions;

public static class ServiceRegistration
{
    public static IServiceCollection AddMonitoring(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        Action<IHealthChecksBuilder>? configureHealthChecks = null,
        Action<IOpenTelemetryBuilder>? configureOpenTelemetry = null)
    {
        // Manifest
        string applicationName = configuration["Manifest:Name"]
            ?? Assembly.GetEntryAssembly()?.GetName().Name
            ?? "UnknownApp";

        var env = configuration["Manifest:Environment"]
            ?? environment.EnvironmentName;

        var systemStartup = TimeProvider.System.GetUtcNow();

        services.AddSingleton(new Manifest()
        {
            Environment = env,
            Name = applicationName,
            SystemStartup = systemStartup
        });

        // Health checks
        var healthChecksBuilder = services.AddHealthChecks();
        configureHealthChecks?.Invoke(healthChecksBuilder);

        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        // Open telemetry
        services.AddSingleton<ManifestResourceDetector>();
        var openTelemetryBuilder = services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder =>
            {
                resourceBuilder.AddDetector((provider) => provider.GetRequiredService<ManifestResourceDetector>());
            });

        openTelemetryBuilder.UseFunctionsWorkerDefaults();

        openTelemetryBuilder.WithTracing(tracingBuilder =>
        {
            tracingBuilder.AddAspNetCoreInstrumentation();
            tracingBuilder.AddHttpClientInstrumentation();
        });

        openTelemetryBuilder.WithMetrics(metricsBuilder =>
        {
            metricsBuilder.AddAspNetCoreInstrumentation();
            metricsBuilder.AddHttpClientInstrumentation();
        });

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });
        });

        var useAzureMonitor = !string.IsNullOrEmpty(configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
        if (useAzureMonitor)
        {
            openTelemetryBuilder.UseAzureMonitor();
        }

        var useOtlpExporter = !string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        if (useOtlpExporter)
        {
            openTelemetryBuilder.UseOtlpExporter();
        }

        configureOpenTelemetry?.Invoke(openTelemetryBuilder);

        return services;
    }
}