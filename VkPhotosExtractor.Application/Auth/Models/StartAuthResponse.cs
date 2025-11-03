namespace VkPhotosExtractor.Application.Auth.Models;

public class StartAuthResponse
{
    public string BaseUrl { get; init; }
    public string Endpoint { get; init; }
    public string ResponseType { get; init; }
    public int ClientId { get; init; }
    public string CodeChallenge { get; init; }
    public string CodeChallengeMethod { get; init; }
    public Uri RedirectUri { get; init; }
    public string State { get; init; }
    public string[]? Scope { get; init; }

    public StartAuthResponse(string baseUrl,
        string endpoint,
        string responseType,
        int clientId,
        string codeChallenge,
        string codeChallengeMethod,
        Uri redirectUri,
        string state,
        string[]? scope)
    {
        BaseUrl = baseUrl;
        Endpoint = endpoint;
        ResponseType = responseType;
        ClientId = clientId;
        CodeChallenge = codeChallenge;
        CodeChallengeMethod = codeChallengeMethod;
        RedirectUri = redirectUri;
        State = state;
        Scope = scope;
    }
}