using VkPhotosExtractor.Application.Auth.Models;
using VkPhotosExtractor.Application.Cache;
using VkPhotosExtractor.Application.Configurations;
using VkPhotosExtractor.Application.Exceptions;

namespace VkPhotosExtractor.Application.Auth;

public class AuthService : IAuthService
{    
    private const int StateLength = 32;
    private const int PkceLength = 64;

    private readonly IVkIdClient _vkIdClient;
    private readonly IConfigurationsProvider _configurationsProvider;
    private readonly ISecurityStringProvider _securityStringProvider;
    private readonly IUserCacheService _userCacheService;

    public AuthService(IVkIdClient vkIdClient, IConfigurationsProvider configurationsProvider, ISecurityStringProvider securityStringProvider, IUserCacheService userCacheService)
    {        
        _vkIdClient = vkIdClient;
        _configurationsProvider = configurationsProvider;
        _securityStringProvider = securityStringProvider;
        _userCacheService = userCacheService;
    }

    public StartAuthResponse GetVkAuthParams(string redirectUrl)
    {
        var returnUri = Uri.EscapeDataString(redirectUrl);

        var vkAppId = _configurationsProvider.GetVkAppId();
        var (state, codeChallenge) = _securityStringProvider.GenerateSecurityStrings(StateLength, PkceLength);

        var startAuthResponse = _vkIdClient.GetAuthParams(vkAppId, state, codeChallenge, returnUri);

        return startAuthResponse;

    }

    public async Task<(Guid userId, DateTime tokenExpiresAt)?> ObtainAccessToken(string usedState, string code, string deviceId, string redirectUrl,
        CancellationToken ct)
    {
        var returnUri = new Uri(redirectUrl);
        var vkAppId = _configurationsProvider.GetVkAppId();
        var state = _securityStringProvider.GenerateRandomString(StateLength);

        var codeVerifier = _securityStringProvider.GetCodeVerifier(usedState);
        if (string.IsNullOrEmpty(codeVerifier))
        {
            return null;
        }

        var authRequest = new AuthRequest(code, codeVerifier, vkAppId, deviceId, returnUri, state);

        var authResponse = await _vkIdClient.ExchangeForAccessToken(authRequest, ct);

        var user = new User(authResponse.UserId,
            deviceId,
            authResponse.AccessToken,
            authResponse.RefreshToken,
            authResponse.IdToken,
            DateTime.UtcNow.Add(authResponse.ExpiresIn));
        
        _userCacheService.CreateOrUpdate(user);
        _securityStringProvider.ClearStateAndCodeVerifier(usedState);

        return (user.Id, user.TokenExpiresAt);
    }

    public async Task<bool> TryRefreshAccessToken(Guid userId, CancellationToken ct)
    {
        var vkAppId = _configurationsProvider.GetVkAppId();
        var user = _userCacheService.TryGetUser(userId);
        if (user is null)
        {
            return false;
        }

        var state = _securityStringProvider.GenerateRandomString(StateLength);

        var refreshRequest = new RefreshTokenRequest(user.RefreshToken,
            vkAppId,
            user.DeviceId,
            state);
        var refreshResponse = await _vkIdClient.RefreshAccessToken(refreshRequest, ct);

        user.Update(refreshResponse.AccessToken,
            refreshResponse.RefreshToken,
            DateTime.UtcNow.Add(refreshResponse.ExpiresIn));
        _userCacheService.CreateOrUpdate(user);

        return true;
    }

    public async Task Logout(Guid userId, CancellationToken ct)
    {
        var vkAppId = _configurationsProvider.GetVkAppId();
        var user = _userCacheService.TryGetUser(userId);
        if (user is null)
        {
            throw new InnerApplicationException("User not found", InnerErrorCode.BadRequest, 400);
        }

        var revokeStatus = await _vkIdClient.RevokeAccessToken(user.AccessToken,
            vkAppId,
            ct);
        if (!revokeStatus)
        {
            throw new ExternalApplicationException("Failed to revoke access token, try again later",
                ExternalErrorCode.VkIdBadGateway,
                502);
        }

        var logoutStatus = await _vkIdClient.Logout(user.AccessToken,
            vkAppId,
            ct);
        if (!logoutStatus)
        {
            throw new ExternalApplicationException("Failed to logout, try again later",
                ExternalErrorCode.VkIdBadGateway,
                502);
        }
        
        _userCacheService.InvalidateUser(user.Id);
    }
}