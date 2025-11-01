using System.Net.Http.Json;
using VkPhotosExtractor.Application;
using VkPhotosExtractor.Application.Auth.Models;

namespace VkPhotosExtractor.Integration.VkClient;

public class VkApiClient : IVkApiClient
{
    private const string VkAuthBaseUrl = "https://id.vk.ru/";
    private const string VkAuthEndpoint = "authorize";
    private const string TokenEndpoint = "oauth2/auth";
    
    private readonly IHttpClientFactory _httpClientFactory;

    public VkApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    public VkStartAuthRequest CreateVkAuthRequest(int vkAppId, string state, string codeChallenge, Uri returnUri)
    {
        return new VkStartAuthRequest(VkAuthBaseUrl,
            VkAuthEndpoint,
            "code",
            vkAppId,
            codeChallenge,
            "S256",
            returnUri,
            state,
            ["vkid.personal_info"]);
    }

    public async Task<VkAuthResponse> ExchangeForAccessToken(string grantType, string code, string codeVerifier, int clientId, string deviceId,
        Uri redirectUri, string state)
    {
        using var client = _httpClientFactory.CreateClient("vk");

        var parameters = new[]
        {
            new KeyValuePair<string, string>("grant_type", grantType),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("code_verifier", codeVerifier),
            new KeyValuePair<string, string>("client_id", clientId.ToString()),
            new KeyValuePair<string, string>("device_id", deviceId),
            new KeyValuePair<string, string>("redirect_uri", redirectUri.ToString()),
            new KeyValuePair<string, string>("state", state)
        };
        var content = new FormUrlEncodedContent(parameters);

        var response = await client.PostAsync(TokenEndpoint, content);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadFromJsonAsync<VkAuthResponse>();
        
        return responseContent;

    }

    public VkAuthResponse RefreshAccessToken(string grantType, string refreshToken, int clientId, string deviceId, string state)
    {
        throw new NotImplementedException();
    }
}