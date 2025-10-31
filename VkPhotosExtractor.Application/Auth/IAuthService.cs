using VkPhotosExtractor.Application.Auth.Models;

namespace VkPhotosExtractor.Application.Auth;

public interface IAuthService
{
    
    VkAuthParams GetVkAuthQueryParams(string redirectUrl);
    bool CheckIfAuthProcessExists(string state);
    Task<VkAuthResponse?> ObtainAccessToken(string state, string code, string deviceId, string redirectUrl);
}