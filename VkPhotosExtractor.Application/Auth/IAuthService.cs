using VkPhotosExtractor.Application.Auth.Models;

namespace VkPhotosExtractor.Application.Auth;

public interface IAuthService
{
    StartAuthResponse CreateVkAuthRequest(string redirectUrl);
    Task<(Guid userId, DateTime tokenExpiresAt)?> ObtainAccessToken(string usedState, string code, string deviceId,
        string redirectUrl,
        CancellationToken ct);

    Task<bool> TryRefreshAccessToken(Guid userId, CancellationToken ct);
    
    Task Logout(Guid userId, CancellationToken ct);
}