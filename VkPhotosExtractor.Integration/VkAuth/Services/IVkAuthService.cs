using VkPhotosExtractor.Integration.VkAuth.Models;

namespace VkPhotosExtractor.Integration.VkAuth.Services;

public interface IVkAuthService
{
    
    VkAuthRequest CreateVkAuthRequest(string redirectUrl);
    Task<VkAuthResponse?> ObtainAccessToken(string state, string code, string deviceId, string redirectUrl);
}