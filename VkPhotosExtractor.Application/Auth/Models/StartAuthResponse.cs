namespace VkPhotosExtractor.Application.Auth.Models;

public class StartAuthResponse
{
    public int VkAppId { get; init; }
    public string FrontendRedirectUrl { get; init; }
    public string State { get; init; }
    public string CodeChallenge { get; init; }
    public string AuthRequestUri { get; init; }

    public StartAuthResponse(int vkAppId,
        string frontendRedirectUrl,
        string state,
        string codeChallenge, 
        string authRequestUri)
    {
        VkAppId = vkAppId;
        FrontendRedirectUrl = frontendRedirectUrl;
        State = state;
        CodeChallenge = codeChallenge;
        AuthRequestUri = authRequestUri;
    }
}