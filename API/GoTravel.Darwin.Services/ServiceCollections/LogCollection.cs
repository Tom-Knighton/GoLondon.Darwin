using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SerilogTracing;
using SerilogTracing.Sinks.OpenTelemetry;

namespace GoTravel.Darwin.Services.ServiceCollections;

public static class LogCollection
{
    public static IServiceCollection AddLogCollection(this IServiceCollection services)
    {
        var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        Serilog.Debugging.SelfLog.Enable((error) => { Console.WriteLine(error);});

        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Service", "GTDAR")
            .Enrich.WithProperty("env", environmentName)
            .Enrich.FromLogContext()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.Seq("https://logs.tomk.online", apiKey: "sM5oXDfEr8X23sgLa2Tk")
            // .WriteTo.OpenTelemetry("https://logs.tomk.online/ingest/otlp/v1/logs", "https://logs.tomk.online/ingest/otlp/v1/traces", OtlpProtocol.HttpProtobuf, null, new Dictionary<string, object>()
            // {
            //     { "service.name", "GTDAR" }
            // })
            .CreateLogger();

        services.AddLogging(l => l.AddSerilog());
        
        // using var _ = new ActivityListenerConfiguration().TraceToSharedLogger();
        Log.Logger.Information("Started");
        return services;
    }
}