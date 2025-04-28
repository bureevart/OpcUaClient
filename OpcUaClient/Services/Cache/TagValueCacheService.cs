using Microsoft.Extensions.Caching.Memory;
using OpcUaClient.Domain.Interfaces.Services;
using OpcUaClient.Domain.Models;
using OpcUaClient.Services.Cache.Configurations;

namespace OpcUaClient.Services.Cache;

public class TagCacheService(IMemoryCache memoryCache, CacheSettings settings)
    : CacheServiceBase(memoryCache, settings),
    ITagCacheService
{
    private static readonly string _tagPrefix = "Tag_";
    
    public Tag? GetTag(string displayName)
    {
        var cacheKey = $"{_tagPrefix}{displayName}";
        if (_cache.TryGetValue(cacheKey, out Tag? value))
            return value;

        return null;
    }

    public void SetTag(string displayName, Tag tag)
    {
        var cacheKey = $"{_tagPrefix}{displayName}";

        _cache.Set(
            cacheKey,
            tag,
            TimeSpan.FromMinutes(_settings.DefaultExpirationTimeInMinutes));
    }
}
