using JBF.Monitoring;
using JBF.Monitoring.AspNetCoreTests.Checks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMonitoring(
    configuration: builder.Configuration,
    environment: builder.Environment,
    configureHealthChecks: b =>
    {
        b.AddCheck<Healthy>(nameof(Healthy));
        b.AddCheck<Degraded>(nameof(Degraded));
        b.AddCheck<Unhealthy>(nameof(Unhealthy));
    });

var app = builder.Build();
app.UseMonitoring();

app.Run();