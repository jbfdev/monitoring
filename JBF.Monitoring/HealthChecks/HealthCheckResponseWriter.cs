using JBF.Core.Monitoring.HealthChecks;
using JBF.Core.Monitoring.Manifests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JBF.Core.Monitoring.AspNetCore.HealthChecks;

public static class HealthCheckResponseWriter
{
    private const string DEFAULT_CONTENT_TYPE = "application/json";
    private static readonly byte[] _emptyResponse = "{}"u8.ToArray();
    private static readonly JsonSerializerOptions _serializerOptions = CreateJsonOptions();

    public static async Task WriteHealthCheckResponse(HttpContext httpContext, HealthReport report)
    {
        var manifest = httpContext.RequestServices.GetRequiredService<Manifest>();

        if (report != null)
        {
            httpContext.Response.ContentType = DEFAULT_CONTENT_TYPE;

            var healthCheckResponse = HealthReportMapper.CreateFrom(manifest, report);

            await JsonSerializer.SerializeAsync(httpContext.Response.Body, healthCheckResponse, _serializerOptions);
        }
        else
        {
            await httpContext.Response.BodyWriter.WriteAsync(_emptyResponse);
        }
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());

        return options;
    }
}