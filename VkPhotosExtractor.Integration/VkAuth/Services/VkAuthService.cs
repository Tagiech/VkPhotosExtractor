using System.Security.Cryptography;
using System.Text;
using VkPhotosExtractor.Application.Configurations;
using VkPhotosExtractor.Cache;
using VkPhotosExtractor.Integration.VkAuth.Models;

namespace VkPhotosExtractor.Integration.VkAuth.Services;

public class VkAuthService : IVkAuthService
{    
    private const int StateLength = 32;
    private const int PkceLength = 64;
    private const string VkAuthBaseUrl = "https://id.vk.ru/";
    private const string VkAuthEndpoint = "authorize";
    private const string AvailableForEncodeChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";


    private readonly int _vkAppId;
    private readonly IPkceCacheService _pkceCacheService;

    public VkAuthService(IConfigurationsProvider configurationsProvider, IPkceCacheService pkceCacheService)
    {
        var vkAppId = configurationsProvider.GetVkAppId();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(vkAppId);
        _vkAppId = vkAppId;
        _pkceCacheService = pkceCacheService;
    }

    public VkAuthRequest GetVkAuthQueryParams(string redirectUrl)
    {
        var state = GenerateRandomString(StateLength);
        var pkce = GeneratePkcePair();
        var returnUri = new Uri(redirectUrl);
        
        _pkceCacheService.Store(state, pkce.codeVerifier);
        
        return new VkAuthRequest(VkAuthBaseUrl,
            VkAuthEndpoint,
            "code",
            _vkAppId,
            pkce.codeChallenge,
            "S256",
            returnUri,
            state,
            ["vkid.personal_info"],
            null,
            null);

    }

    public bool CheckIfAuthProcessExists(string state)
    {
        return _pkceCacheService.TryGetValue(state, out _);
    }

    public Task<VkAuthResponse?> ObtainAccessToken(string state, string code, string deviceId, string redirectUrl)
    {
        return Task.FromResult(new VkAuthResponse("", "", "", "", 600, 155, "", []))!;
    }

    private static string GenerateRandomString(int length)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        return new string(bytes.Select(b => AvailableForEncodeChars[b % AvailableForEncodeChars.Length]).ToArray());
    }

    private static (string codeVerifier, string codeChallenge) GeneratePkcePair()
    {
        var codeVerifier = GenerateRandomString(PkceLength);

        // code_challenge: BASE64-ENCODE(SHA256(codeVerifier))
        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        var base64 = Convert.ToBase64String(hash);
        var codeChallenge = base64.Replace("+", "-").Replace("/", "_").Replace("=", "");

        return (codeVerifier, codeChallenge);
    }
}