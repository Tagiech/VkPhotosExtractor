using VkPhotosExtractor.Application.Auth.Models;
using VkPhotosExtractor.Application.Configurations;

namespace VkPhotosExtractor.Application.Auth;

public class VkAuthService : IVkAuthService
{    
    private const int StateLength = 32;
    private const int PkceLength = 64;

    private readonly IVkApiClient _vkApiClient;
    private readonly IConfigurationsProvider _configurationsProvider;
    private readonly ISecurityStringProvider _securityStringProvider;

    public VkAuthService(IVkApiClient vkApiClient, IConfigurationsProvider configurationsProvider, ISecurityStringProvider securityStringProvider)
    {        
        _vkApiClient = vkApiClient;
        _configurationsProvider = configurationsProvider;
        _securityStringProvider = securityStringProvider;
    }

    public VkStartAuthRequest CreateVkAuthRequest(string redirectUrl)
    {
        var returnUri = new Uri(redirectUrl);
        var vkAppId = _configurationsProvider.GetVkAppId();
        var (state, codeChallenge) = _securityStringProvider.GenerateSecurityStrings(StateLength, PkceLength);
        
        var startAuthRequest = _vkApiClient.CreateVkAuthRequest(vkAppId, state, codeChallenge, returnUri);

        return startAuthRequest;

    }

    public Task<VkAuthResponse?> ObtainAccessToken(string state, string code, string deviceId, string redirectUrl)
    {
        return Task.FromResult(new VkAuthResponse("", "", "", "", TimeSpan.FromHours(1), 155, "", []))!;
    }
}