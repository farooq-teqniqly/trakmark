using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SchoolTrackTracker.Extensions;

public static class TelemetryExtensions
{
    private const string ServiceName = "SchoolTrackTracker";

    public static IServiceCollection AddAppTelemetry(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        var otlpEndpoint = config["OpenTelemetry:Endpoint"] ?? "http://localhost:4317";

        services
            .AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(ServiceName))
            .WithTracing(tracing =>
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint))
            )
            .WithMetrics(metrics =>
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint))
            );

        services.AddLogging(logging =>
            logging.AddOpenTelemetry(o =>
            {
                o.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName));
                o.AddOtlpExporter(e => e.Endpoint = new Uri(otlpEndpoint));
            })
        );

        return services;
    }
}
