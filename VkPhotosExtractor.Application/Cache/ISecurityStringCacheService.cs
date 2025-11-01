namespace VkPhotosExtractor.Application.Cache;

public interface ISecurityStringCacheService
{
    void Store(string key, string value);
    bool TryGetValue(string key, out string? value);
    void Invalidate(string key);
}