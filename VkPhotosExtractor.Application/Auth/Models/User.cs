namespace VkPhotosExtractor.Application.Auth.Models;

public class User
{
    public Guid Id { get; set; }
    public int ExternalId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string IdToken { get; set; }
    public DateTime TokenExpiresAt { get; set; }
    
    public User(int externalId,
        string accessToken,
        string refreshToken,
        string idToken,
        DateTime tokenExpiresAt)
    {
        Id = Guid.NewGuid();
        ExternalId = externalId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        IdToken = idToken;
        TokenExpiresAt = tokenExpiresAt;
    }
}