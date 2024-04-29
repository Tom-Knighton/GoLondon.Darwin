using GoTravel.Darwin.Domain.Interfaces;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoTravel.Darwin.Services.ServiceCollections;

public static class HangfireCollection
{

    public static IServiceCollection AddHangfireCollection(this IServiceCollection services, IConfiguration configuration)
    {
        var migrationOptions = new MongoMigrationOptions()
        {
            MigrationStrategy = new DropMongoMigrationStrategy(),
            BackupStrategy = new CollectionMongoBackupStrategy()
        };
        var storageOptions = new MongoStorageOptions
        {
            MigrationOptions = migrationOptions,
            CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
        };

        services.AddHangfire(cfg =>
        {
            cfg.UseSimpleAssemblyNameTypeSerializer();
            cfg.UseRecommendedSerializerSettings();
            cfg.UseMongoStorage(configuration["Host"], storageOptions);
        });

        services.AddHangfireServer();
        
        
        return services;
    }
    
     public static IApplicationBuilder UseHangfire(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireCustomBasicAuthenticationFilter
            {
                User = configuration["AdminUser"],
                Pass = configuration["AdminPassword"]
            } }
        });

        app.UseEndpoints(e => e.MapHangfireDashboard());

        RecurringJob.AddOrUpdate<IDarwinConsumer>("WatchPushPort", x => x.Start(), Cron.Never());

        return app;
    }
}