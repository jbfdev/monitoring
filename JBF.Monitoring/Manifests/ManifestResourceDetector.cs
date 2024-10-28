using OpenTelemetry.Resources;

namespace JBF.Monitoring.Manifests;

public class ManifestResourceDetector(Manifest manifest) : IResourceDetector
{
    public Resource Detect()
    {
        Dictionary<string, object> attributes = new()
        {
            { "OTEL_SERVICE_NAME", manifest.Name },
            { "OTEL_RESOURCE_ATTRIBUTES",  $"environment.name={manifest.Environment}"},
            { "service.name", manifest.Name },
            { "deployment.environment",  manifest.Environment},
        };

        return new Resource(attributes);
    }
}
