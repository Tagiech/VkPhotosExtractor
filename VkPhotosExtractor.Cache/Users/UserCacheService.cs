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

    public void CreateOrUpdate(User user)
    {
        _memoryCache.Set(CachePrefix + user.Id, user, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
        });
    }

    public User? TryGetUser(Guid userId)
    {
        var userExists = _memoryCache.TryGetValue(CachePrefix + userId, out User? user);
        return !userExists ? null : user;
    }

    public void InvalidateUser(Guid userId)
    {
        _memoryCache.Remove(CachePrefix + userId);
    }
}