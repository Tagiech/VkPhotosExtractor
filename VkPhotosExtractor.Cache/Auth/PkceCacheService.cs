using Microsoft.Extensions.Caching.Memory;

namespace VkPhotosExtractor.Cache;

public class PkceCacheService : IPkceCacheService
{
    private readonly IMemoryCache _memoryCache;

    public PkceCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void Store(string state, string codeVerifier)
    {
        _memoryCache.Set($"state:{state}", codeVerifier, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
    }

    public bool TryGetValue(string state, out string? codeVerifier)
    {
        return _memoryCache.TryGetValue($"state:{state}", out codeVerifier);
    }
}