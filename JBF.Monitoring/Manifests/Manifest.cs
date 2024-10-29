namespace JBF.Monitoring.Manifests;

public class Manifest
{
    public required DateTimeOffset SystemStartup { get; init; }
    public required string Name { get; init; }
    public required string Environment { get; init; }
}