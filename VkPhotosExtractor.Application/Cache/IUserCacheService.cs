using VkPhotosExtractor.Application.Auth.Models;

namespace VkPhotosExtractor.Application.Cache;

public interface IUserCacheService
{
    void CreateOrUpdate(User user);
    bool TryGetUser(Guid userId, out User? user);
    void InvalidateUser(Guid userId);
}