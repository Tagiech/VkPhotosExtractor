using VkPhotosExtractor.Application.Auth.Models;

namespace VkPhotosExtractor.Application.Cache;

public interface IUserCacheService
{
    void CreateOrUpdate(User user);
    User? TryGetUser(Guid userId);
    void InvalidateUser(Guid userId);
}