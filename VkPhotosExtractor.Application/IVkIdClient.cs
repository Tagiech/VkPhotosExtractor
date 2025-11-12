using VkPhotosExtractor.Application.Auth.Models;

namespace VkPhotosExtractor.Application;

public interface IVkIdClient
{
    StartAuthResponse GetAuthParams(int vkAppId, string state, string codeChallenge, string redirectUrl);
    Task<AuthResponse> ExchangeForAccessToken(AuthRequest request, CancellationToken ct);

    Task<RefreshTokenResponse> RefreshAccessToken(RefreshTokenRequest request, CancellationToken ct);
    
    Task<bool> RevokeAccessToken(string accessToken,
        int clientId,
        CancellationToken ct);
    
    Task<bool> Logout(string accessToken,
        int clientId,
        CancellationToken ct);

    Task<UserInfo> GetUserInfo(Guid userId, string accessToken, int clientId, CancellationToken ct);
}