using VkPhotosExtractor.Application.Auth.Models;

namespace VkPhotosExtractor.Application.Auth;

public interface IVkAuthService
{
    VkStartAuthRequest CreateVkAuthRequest(string redirectUrl);
    Task<VkAuthResponse?> ObtainAccessToken(string state, string code, string deviceId, string redirectUrl);
}