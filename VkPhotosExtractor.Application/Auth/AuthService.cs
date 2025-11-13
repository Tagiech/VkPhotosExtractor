using System.Text;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<AuthService> _logger;

    public AuthService(IVkIdClient vkIdClient,
        IConfigurationsProvider configurationsProvider,
        ISecurityStringProvider securityStringProvider,
        IUserCacheService userCacheService,
        ILogger<AuthService> logger)
    {        
        _vkIdClient = vkIdClient;
        _configurationsProvider = configurationsProvider;
        _securityStringProvider = securityStringProvider;
        _userCacheService = userCacheService;
        _logger = logger;
    }

    public StartAuthResponse GetVkAuthParams()
    {
        var redirectUrl = GetEncodedFrontendRedirectUrl();
        var vkAppId = _configurationsProvider.GetVkAppId();
        var (state, codeChallenge) = _securityStringProvider.GenerateSecurityStrings(StateLength, PkceLength);

        var startAuthResponse = _vkIdClient.GetAuthParams(vkAppId, state, codeChallenge, redirectUrl);
        _logger.LogInformation("GetVkAuthParams returned: {@StartAuthResponse}", 
            startAuthResponse);

        return startAuthResponse;
    }

    public async Task<(Guid userId, DateTime tokenExpiresAt)?> ObtainAccessToken(string usedState, string code, string deviceId, string redirectUrl,
        CancellationToken ct)
    {
        var returnUri = new Uri(redirectUrl);
        var vkAppId = _configurationsProvider.GetVkAppId();
        var state = _securityStringProvider.GenerateRandomString(StateLength);

        var codeVerifier = _securityStringProvider.GetCodeVerifier(usedState);
        _logger.LogInformation("""
                               Method: ObtainAccessToken
                               Obtaining access token with
                               state: {@state},
                               code: {@code},
                               deviceId: {@deviceId},
                               redirectUrl: {@redirectUrl}, 
                               codeVerifier: {@codeVerifier}
                               """,
            usedState, code, deviceId, redirectUrl, codeVerifier);
        if (string.IsNullOrEmpty(codeVerifier))
        {
            return null;
        }

        var authRequest = new AuthRequest(code, codeVerifier, vkAppId, deviceId, returnUri, state);

        var authResponse = await _vkIdClient.ExchangeForAccessToken(authRequest, ct);
        _logger.LogInformation("""
                               Method: ObtainAccessToken
                               Exchanged code for access token with
                               request: {@authRequest},
                               response: {@authResponse}
                               """,
            authRequest, authResponse);

        var user = new User(authResponse.UserId,
            deviceId,
            authResponse.AccessToken,
            authResponse.RefreshToken,
            authResponse.IdToken,
            DateTime.UtcNow.Add(authResponse.ExpiresIn));
        
        _userCacheService.CreateOrUpdate(user);
        _logger.LogInformation("""
                               Method: ObtainAccessToken
                               Created or updated user in cache: {@user}
                               """,
            user);
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

    public async Task<UserInfo> GetUserInfo(Guid userId, CancellationToken ct)
    {
        var user = _userCacheService.TryGetUser(userId);
        _logger.LogInformation("""
                               Method: GetUserInfo
                               Got user from cache:
                               UserId: {@userId}
                               User from cache: {@user}
                               """,
            userId, user);
        if (user is null)
        {
            throw new InnerApplicationException("User not found", InnerErrorCode.BadRequest, 400);
        }
        var vkAppId = _configurationsProvider.GetVkAppId();
        //TODO: use cache through policy here
        var userInfo = await _vkIdClient.GetUserInfo(userId, user.AccessToken, vkAppId, ct);
        _logger.LogInformation("""
                               Method: GetUserInfo
                               Got user info from VK ID:
                               User: {@user}
                               User info: {@userInfo}
                               """,
            user, userInfo);
        
        return userInfo;
    }

    private string GetEncodedFrontendRedirectUrl()
    {
        var redirectUrlBuilder = new StringBuilder(_configurationsProvider.GetFrontendHost());
        redirectUrlBuilder.Append("/Auth/callback");
        
        return Uri.EscapeDataString(redirectUrlBuilder.ToString());
    }
}