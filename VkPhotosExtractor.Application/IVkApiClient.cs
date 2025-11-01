using VkPhotosExtractor.Application.Auth.Models;

namespace VkPhotosExtractor.Application;

public interface IVkApiClient
{
    VkStartAuthRequest CreateVkAuthRequest(int vkAppId, string state, string codeChallenge, Uri returnUri);
    Task<VkAuthResponse> ExchangeForAccessToken(string grantType,
        string code,
        string codeVerifier,
        int clientId,
        string deviceId,
        Uri redirectUri,
        string state);

    VkAuthResponse RefreshAccessToken(string grantType,
        string refreshToken,
        int clientId,
        string deviceId,
        string state);

}