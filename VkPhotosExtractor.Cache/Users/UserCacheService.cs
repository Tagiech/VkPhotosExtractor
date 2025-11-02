using Microsoft.Extensions.Caching.Memory;
using VkPhotosExtractor.Application.Auth.Models;
using VkPhotosExtractor.Application.Cache;

namespace VkPhotosExtractor.Cache.Users;

public class UserCacheService : IUserCacheService
{
    private const string CachePrefix = "User:";
    private readonly IMemoryCache _memoryCache;

    public UserCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void SaveUser(User user)
    {
        _memoryCache.Set(CachePrefix + user.Id, user, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
        });
    }

    public void UpdateUser(User user)
    {
        throw new NotImplementedException();
    }

    public bool TryGetUser(Guid userId, out User? user)
    {
        throw new NotImplementedException();
    }

    public void InvalidateUser(Guid userId)
    {
        throw new NotImplementedException();
    }
}