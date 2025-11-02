namespace VkPhotosExtractor.Application.Auth.Models;

public class AuthRequest
{
    public string Code { get; init; }
    public string CodeVerifier { get; init; }
    public int ClientId { get; init; }
    public string DeviceId { get; init; }
    public Uri RedirectUri { get; init; }
    public string State { get; init; }

    public AuthRequest(string code, string codeVerifier, int clientId, string deviceId, Uri redirectUri, string state)
    {
        Code = code;
        CodeVerifier = codeVerifier;
        ClientId = clientId;
        DeviceId = deviceId;
        RedirectUri = redirectUri;
        State = state;
    }
}