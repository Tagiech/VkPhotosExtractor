namespace VkPhotosExtractor.Integration.VkAuth.Models;

public class VkAuthRequest
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
    public VkLangId? LangId { get; init; }
    public VkAuthScheme? Scheme { get; init; }

    public VkAuthRequest(string baseUrl,
        string endpoint,
        string responseType,
        int clientId,
        string codeChallenge,
        string codeChallengeMethod,
        Uri redirectUri,
        string state,
        string[]? scope,
        VkLangId? langId,
        VkAuthScheme? scheme)
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
        LangId = langId;
        Scheme = scheme;
    }
}