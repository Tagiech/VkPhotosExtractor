namespace VkPhotosExtractor.Application.Auth.Models;

public class User
{
    public Guid Id { get; private set; }
    public int ExternalId { get; private set; }
    public string DeviceId { get; private set; }
    public string AccessToken { get; private set; }
    public string RefreshToken { get; private set; }
    public string IdToken { get; private set; }
    public DateTime TokenExpiresAt { get; private set; }
    
    public User(int externalId,
        string deviceId,
        string accessToken,
        string refreshToken,
        string idToken,
        DateTime tokenExpiresAt)
    {
        Id = Guid.NewGuid();
        ExternalId = externalId;
        DeviceId = deviceId;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        IdToken = idToken;
        TokenExpiresAt = tokenExpiresAt;
    }

    public void Update(string accessToken,
        string refreshToken,
        DateTime tokenExpiresAt)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        TokenExpiresAt = tokenExpiresAt;
    }
}