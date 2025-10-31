namespace VkPhotosExtractor.Cache;

public interface IPkceCacheService
{
    void Store(string state, string codeVerifier);
    bool TryGetValue(string state, out string? codeVerifier);
}