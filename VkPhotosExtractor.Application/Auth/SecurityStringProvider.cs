using System.Security.Cryptography;
using System.Text;
using VkPhotosExtractor.Application.Cache;

namespace VkPhotosExtractor.Application.Auth;

public class SecurityStringProvider : ISecurityStringProvider
{
    private const string AvailableForEncodeChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private readonly ISecurityStringCacheService _securityStringCacheService;

    public SecurityStringProvider(ISecurityStringCacheService securityStringCacheService)
    {
        _securityStringCacheService = securityStringCacheService;
    }

    public (string state, string codeChallenge) GenerateSecurityStrings(int stateLength, int codeChallengeLength)
    {
        var state = GenerateRandomString(stateLength);
        var (codeVerifier, codeChallenge) = GeneratePkcePair(codeChallengeLength);
        
        _securityStringCacheService.Store(state, codeVerifier);

        return (state, codeChallenge);

    }

    public string? GetCodeVerifier(string state) => 
        _securityStringCacheService.TryGetValue(state, out var codeVerifier) ? codeVerifier : null;

    public void ClearCodeVerifier(string state) => 
        _securityStringCacheService.Invalidate(state);

    private static string GenerateRandomString(int length)
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        return new string(bytes.Select(b => AvailableForEncodeChars[b % AvailableForEncodeChars.Length]).ToArray());
    }

    private static (string codeVerifier, string codeChallenge) GeneratePkcePair(int length)
    {
        var codeVerifier = GenerateRandomString(length);

        // code_challenge: BASE64-ENCODE(SHA256(codeVerifier))
        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        var base64 = Convert.ToBase64String(hash);
        var codeChallenge = base64.Replace("+", "-").Replace("/", "_").Replace("=", "");

        return (codeVerifier, codeChallenge);
    }
}