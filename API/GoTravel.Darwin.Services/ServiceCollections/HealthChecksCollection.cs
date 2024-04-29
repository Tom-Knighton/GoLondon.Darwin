using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GoTravel.Darwin.Services.ServiceCollections;

public static class HealthChecksCollection
{
    public static IServiceCollection AddHealthChecksCollection(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("OK", () => HealthCheckResult.Healthy())
            .AddCheck("Unhealthy", () => HealthCheckResult.Unhealthy(":("));
        
        services.AddHealthChecksUI(o =>
        {
            o.SetEvaluationTimeInSeconds(10);
            o.MaximumHistoryEntriesPerEndpoint(120);
        })
        .AddInMemoryStorage();
        
        return services;
    }

    public static IApplicationBuilder UseHealthChecksCollection(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health");
        app.UseHealthChecksUI(o => o.UIPath = "/health-ui");
        
        return app;
    }
}