using VkPhotosExtractor.Application.Auth.Models;

namespace VkPhotosExtractor.Application.Cache;

public interface IUserCacheService
{
    void SaveUser(User user);
    void UpdateUser(User user);
    bool TryGetUser(Guid userId, out User? user);
    void InvalidateUser(Guid userId);
}