using Microsoft.Extensions.Caching.Memory;
using OpcUaClient.Services.Cache.Configurations;

namespace OpcUaClient.Services.Cache;

public class CacheServiceBase(
    IMemoryCache memoryCache,
    CacheSettings settings)
{
    protected readonly IMemoryCache _cache = memoryCache;
    protected readonly CacheSettings _settings = settings;
}
