namespace VkPhotosExtractor.Application.Auth.Models;

public class RefreshTokenRequest
{
    public string RefreshToken { get; init; }
    public int ClientId { get; init; }
    public string DeviceId { get; init; }
    public string State { get; init; }

    public RefreshTokenRequest(string refreshToken, int clientId, string deviceId, string state)
    {
        RefreshToken = refreshToken;
        ClientId = clientId;
        DeviceId = deviceId;
        State = state;
    }
}