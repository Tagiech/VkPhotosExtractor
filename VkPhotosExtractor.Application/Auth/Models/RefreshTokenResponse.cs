namespace VkPhotosExtractor.Application.Auth.Models;

public class RefreshTokenResponse
{
    public string RefreshToken { get; init; }
    public string AccessToken { get; init; }
    public string TokenType { get; init; }
    public TimeSpan ExpiresIn { get; init; }
    public int UserId { get; init; }
    public string[] Scope { get; init; }

    public RefreshTokenResponse(string refreshToken,
        string accessToken,
        string tokenType,
        TimeSpan expiresIn,
        int userId,
        string[] scope)
    {
        RefreshToken = refreshToken;
        AccessToken = accessToken;
        TokenType = tokenType;
        ExpiresIn = expiresIn;
        UserId = userId;
        Scope = scope;
    }
}