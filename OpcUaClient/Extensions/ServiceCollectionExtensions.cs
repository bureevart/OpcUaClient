using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OpcUaClient.DataAccessLayer;
using OpcUaClient.Domain.Interfaces;
using OpcUaClient.Domain.Interfaces.Providers;
using OpcUaClient.Domain.Interfaces.Services;
using OpcUaClient.Domain.Providers;
using OpcUaClient.Domain.Validation;
using OpcUaClient.Services.Cache;
using Serilog;
using Serilog.Events;

namespace OpcUaClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection
        services, IConfiguration configuration, string name)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: name,
                b =>
                {
                    b.WithOrigins(configuration.GetSection("CORS:Origins").Get<string[]>() ?? throw new InvalidOperationException())
                        .WithHeaders(configuration.GetSection("CORS:Headers").Get<string[]>() ?? throw new InvalidOperationException())
                        .WithMethods(configuration.GetSection("CORS:Methods").Get<string[]>() ?? throw new InvalidOperationException());
                });
        });

        return services;
    }
    
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection
        services, IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString("EntityContext");

        services.AddDbContext<ServiceDbContext>(options =>
            options.UseNpgsql(connectionString));
        services.AddScoped<IServiceDbContext>(provider =>
            provider.GetService<ServiceDbContext>() ?? throw new InvalidOperationException());

        services.AddScoped<IServerCrudProvider, ServerCrudProvider>();
        services.AddScoped<ITagCrudProvider, TagCrudProvider>();

        return services;
    }
    
    public static IServiceCollection AddSerilogLogging(this IServiceCollection
        services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Logger(lc => lc
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
            )
            .WriteTo.Logger(lc => lc
                .WriteTo.File($"Logs{Path.DirectorySeparatorChar}OpcUaClientAppLog-.txt", rollingInterval: RollingInterval.Day)
            )
            .CreateLogger();
        
        services.AddSingleton(Log.Logger);
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger);
        });
        
        return services;
    }
    
    public static IServiceCollection AddFluentValidation(this IServiceCollection
        services)
    {
        services.AddValidatorsFromAssemblyContaining<TagValidator>();
        
        return services;
    }
    
    public static void AddCache(this IServiceCollection services,
        IConfiguration configuration)
    {
        var cacheSettings = GetCacheSettings(configuration);
        
        services.AddMemoryCache();

        services.AddSingleton<ITagCacheService, TagCacheService>(provider =>
        {
            var memoryCache = provider.GetRequiredService<IMemoryCache>();
            var memoryCacheService = new TagCacheService(
                memoryCache,
                cacheSettings);

            return memoryCacheService;
        });
    }
    
    private static Services.Cache.Configurations.CacheSettings GetCacheSettings(IConfiguration configuration)
    {
        var cacheService = configuration
                               .GetSection("Cache:CacheService")
                               .Get<Services.Cache.Configurations.CacheSettings>() 
                           ?? throw new Exception();
        
        return cacheService;
    }
}