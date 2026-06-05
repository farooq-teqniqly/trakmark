using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Trakmark.Extensions;

/// <summary>Extension methods that configure OpenTelemetry tracing, metrics, and logging for the application.</summary>
public static class TelemetryExtensions
{
    private const string ServiceName = "Trakmark";

    /// <summary>
    /// Adds OpenTelemetry tracing, metrics, and logging to <paramref name="services"/>,
    /// exporting to the OTLP endpoint specified in configuration (defaults to <c>http://localhost:4317</c>).
    /// </summary>
    public static IServiceCollection AddAppTelemetry(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        var otlpEndpointRaw = config["OpenTelemetry:Endpoint"] ?? "http://localhost:4317";

        if (!Uri.TryCreate(otlpEndpointRaw, UriKind.Absolute, out var otlpEndpoint))
        {
            throw new InvalidOperationException(
                $"Invalid OpenTelemetry:Endpoint '{otlpEndpointRaw}'. Must be an absolute URI (e.g. http://localhost:4317).");
        }

        services
            .AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(ServiceName))
            .WithTracing(tracing =>
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddOtlpExporter(o => o.Endpoint = otlpEndpoint)
            )
            .WithMetrics(metrics =>
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(o => o.Endpoint = otlpEndpoint)
            );

        services.AddLogging(logging =>
            logging.AddOpenTelemetry(o =>
            {
                o.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName));
                o.AddOtlpExporter(e => e.Endpoint = otlpEndpoint);
            })
        );

        return services;
    }
}
