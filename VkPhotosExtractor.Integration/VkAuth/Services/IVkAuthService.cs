using VkPhotosExtractor.Integration.VkAuth.Models;

namespace VkPhotosExtractor.Integration.VkAuth.Services;

public interface IVkAuthService
{
    
    VkAuthRequest GetVkAuthQueryParams(string redirectUrl);
    bool CheckIfAuthProcessExists(string state);
    Task<VkAuthResponse?> ObtainAccessToken(string state, string code, string deviceId, string redirectUrl);
}