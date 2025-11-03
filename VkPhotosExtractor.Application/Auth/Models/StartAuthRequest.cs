namespace VkPhotosExtractor.Application.Auth.Models;

public class StartAuthRequest
{
    public int VkAppId { get; init; }
    public string State { get; init; }
    public string CodeChallenge { get; init; }
    public Uri ReturnUri { get; init; }

    public StartAuthRequest(int vkAppId, string state, string codeChallenge, Uri returnUri)
    {
        VkAppId = vkAppId;
        State = state;
        CodeChallenge = codeChallenge;
        ReturnUri = returnUri;
    }
}