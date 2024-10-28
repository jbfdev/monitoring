using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace JBF.Core.Monitoring.Manifests;

public class Manifest
{
    public required DateTimeOffset SystemStartup { get; set; }
    public required string Name { get; set; }
    public required string Environment { get; set; }
}