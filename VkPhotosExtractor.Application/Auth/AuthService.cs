using VkPhotosExtractor.Application.Auth.Models;
using VkPhotosExtractor.Application.Configurations;

namespace VkPhotosExtractor.Application.Auth;

public class AuthService : IAuthService
{    
    private const int StateLength = 32;
    private const int PkceLength = 64;

    private readonly IVkIdClient _vkIdClient;
    private readonly IConfigurationsProvider _configurationsProvider;
    private readonly ISecurityStringProvider _securityStringProvider;

    public AuthService(IVkIdClient vkIdClient, IConfigurationsProvider configurationsProvider, ISecurityStringProvider securityStringProvider)
    {        
        _vkIdClient = vkIdClient;
        _configurationsProvider = configurationsProvider;
        _securityStringProvider = securityStringProvider;
    }

    public StartAuthResponse CreateVkAuthRequest(string redirectUrl)
    {
        var returnUri = new Uri(redirectUrl);
        var vkAppId = _configurationsProvider.GetVkAppId();
        var (state, codeChallenge) = _securityStringProvider.GenerateSecurityStrings(StateLength, PkceLength);

        var startAuthRequest = new StartAuthRequest(vkAppId, state, codeChallenge, returnUri);
        
        var startAuthResponse = _vkIdClient.CreateVkAuthRequest(startAuthRequest);

        return startAuthResponse;

    }

    public Task<AuthResponse> ObtainAccessToken(string state, string code, string deviceId, string redirectUrl,
        CancellationToken ct)
    {
        return Task.FromResult(new AuthResponse("", "", "", TimeSpan.FromHours(1), 666, [] ));
    }
}