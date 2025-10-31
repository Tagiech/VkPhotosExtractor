using Microsoft.Extensions.Caching.Memory;

namespace VkPhotosExtractor.Cache.Auth;

public class SecurityStringCacheService : ISecurityStringCacheService
{
    private const string CachePrefix = "SecurityString:";
    private readonly IMemoryCache _memoryCache;

    public SecurityStringCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void Store(string key, string value)
    {
        _memoryCache.Set(CachePrefix + key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
    }

    public bool TryGetValue(string key, out string? value)
    {
        return _memoryCache.TryGetValue(CachePrefix + key, out value);
    }

    public void Invalidate(string key)
    {
        _memoryCache.Remove(CachePrefix + key);
    }
}