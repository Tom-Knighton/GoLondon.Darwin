using GoTravel.Darwin.Domain.Interfaces;
using GoTravel.Darwin.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.Darwin.Services.ServiceCollections;

public static class DarwinCollection
{
    public static IServiceCollection AddDarwinServices(this IServiceCollection services)
    {
        services.AddSingleton<IDarwinConsumer, DarwinConsumer>();

        return services;
    }
}