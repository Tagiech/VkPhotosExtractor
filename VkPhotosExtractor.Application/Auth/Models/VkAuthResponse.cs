namespace VkPhotosExtractor.Application.Auth.Models;

public class VkAuthResponse
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public string IdToken { get; init; }
    public string TokenType { get; init; }
    public TimeSpan ExpiresIn { get; init; }
    public int UserId { get; init; }
    public string State { get; init; }
    public string[] Scope { get; init; }

    public VkAuthResponse(string accessToken,
        string refreshToken,
        string idToken,
        string tokenType,
        TimeSpan expiresIn,
        int userId,
        string state,
        string[] scope)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        IdToken = idToken;
        TokenType = tokenType;
        ExpiresIn = expiresIn;
        UserId = userId;
        State = state;
        Scope = scope;
    }
}