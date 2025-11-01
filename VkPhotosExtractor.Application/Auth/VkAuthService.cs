using VkPhotosExtractor.Application.Auth.Models;
using VkPhotosExtractor.Application.Configurations;

namespace VkPhotosExtractor.Application.Auth;

public class VkAuthService : IVkAuthService
{    
    private const int StateLength = 32;
    private const int PkceLength = 64;
    private const string VkAuthBaseUrl = "https://id.vk.ru/";
    private const string VkAuthEndpoint = "authorize";

    private readonly IConfigurationsProvider _configurationsProvider;
    private readonly ISecurityStringProvider _securityStringProvider;

    public VkAuthService(IConfigurationsProvider configurationsProvider, ISecurityStringProvider securityStringProvider)
    {
        _configurationsProvider = configurationsProvider;
        _securityStringProvider = securityStringProvider;
    }

    public VkAuthRequest CreateVkAuthRequest(string redirectUrl)
    {
        var vkAppId = _configurationsProvider.GetVkAppId();
        
        var (state, codeChallenge) = _securityStringProvider.GenerateSecurityStrings(StateLength, PkceLength);
        var returnUri = new Uri(redirectUrl);
        
        return new VkAuthRequest(VkAuthBaseUrl,
            VkAuthEndpoint,
            "code",
            vkAppId,
            codeChallenge,
            "S256",
            returnUri,
            state,
            ["vkid.personal_info"],
            VkLangId.RUS,
            VkAuthScheme.Light);

    }

    public Task<VkAuthResponse?> ObtainAccessToken(string state, string code, string deviceId, string redirectUrl)
    {
        return Task.FromResult(new VkAuthResponse("", "", "", "", TimeSpan.FromHours(1), 155, "", []))!;
    }
}