namespace VkPhotosExtractor.Application.Auth.Models;

public class AuthResponse
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public string IdToken { get; init; }
    public TimeSpan ExpiresIn { get; init; }
    public int UserId { get; init; }
    public string[] Scope { get; init; }

    public AuthResponse(string accessToken,
        string refreshToken,
        string idToken,
        TimeSpan expiresIn,
        int userId,
        string[] scope)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        IdToken = idToken;
        ExpiresIn = expiresIn;
        UserId = userId;
        Scope = scope;
    }
}