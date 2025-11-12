using System.Text;
using System.Text.Json;
using VkPhotosExtractor.Application;
using VkPhotosExtractor.Application.Auth.Models;
using VkPhotosExtractor.Application.Configurations;
using VkPhotosExtractor.Application.Exceptions;
using VkPhotosExtractor.Integration.VkClient.Dto;
using VkPhotosExtractor.Integration.VkClient.Dto.Helpers;

namespace VkPhotosExtractor.Integration.VkClient;

public class VkIdClient : IVkIdClient
{
    private const string VkAuthEndpoint = "/authorize";
    private const string TokenEndpoint = "/oauth2/auth";
    private const string RevokeTokenEndpoint = "/oauth2/revoke";
    private const string LogoutEndpoint = "/oauth2/logout";
    private const string UserInfoEndpoint = "/oauth2/user_info";

    private const string AuthGrantType = "authorization_code";
    private const string RefreshGrantType = "refresh_token";
    
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfigurationsProvider _configurationsProvider;

    public VkIdClient(IHttpClientFactory httpClientFactory, IConfigurationsProvider configurationsProvider)
    {
        _httpClientFactory = httpClientFactory;
        _configurationsProvider = configurationsProvider;
    }


    public StartAuthResponse GetAuthParams(int vkAppId, string state, string codeChallenge, string redirectUrl)
    {
        var builder = new StringBuilder();

        builder.Append(_configurationsProvider.GetVkIdHost());
        builder.Append(VkAuthEndpoint);
        builder.Append("?response_type=code");
        builder.Append($"&client_id={vkAppId}");
        builder.Append($"&redirect_uri={redirectUrl}");
        builder.Append($"&state={state}");
        builder.Append($"&code_challenge={codeChallenge}");
        builder.Append("&code_challenge_method=S256");
        builder.Append("&scope=groups");

        return new StartAuthResponse(vkAppId, redirectUrl, state, codeChallenge, builder.ToString());
    }

    public async Task<AuthResponse> ExchangeForAccessToken(AuthRequest request, CancellationToken ct)
    {
        var parameters = new[]
        {
            new KeyValuePair<string, string>("grant_type", AuthGrantType),
            new KeyValuePair<string, string>("code", request.Code),
            new KeyValuePair<string, string>("code_verifier", request.CodeVerifier),
            new KeyValuePair<string, string>("client_id", request.ClientId.ToString()),
            new KeyValuePair<string, string>("device_id", request.DeviceId),
            new KeyValuePair<string, string>("redirect_uri", request.RedirectUri.ToString()),
            new KeyValuePair<string, string>("state", request.State)
        };
        using var requestContent = new FormUrlEncodedContent(parameters);

        var response = await TryPostRequest(TokenEndpoint, requestContent, ct);
        
        var content = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            ThrowResponseError(content, response);
        }

        var exchangeResult = TryDeserialize<VkAuthResponseDto>(content, response);
        CheckForStateMismatch(request.State, exchangeResult.State, response);

        return exchangeResult.ToAuthResponse();
    }

    public async Task<RefreshTokenResponse> RefreshAccessToken(RefreshTokenRequest request, CancellationToken ct)
    {
        var parameters = new[]
        {
            new KeyValuePair<string, string>("grant_type", RefreshGrantType),
            new KeyValuePair<string, string>("refresh_token", request.RefreshToken),
            new KeyValuePair<string, string>("client_id", request.ClientId.ToString()),
            new KeyValuePair<string, string>("device_id", request.DeviceId),
            new KeyValuePair<string, string>("state", request.State)
        };
        using var requestContent = new FormUrlEncodedContent(parameters);

        var response = await TryPostRequest(TokenEndpoint, requestContent, ct);

        var content = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            ThrowResponseError(content, response);
        }

        var refreshResult = TryDeserialize<VkRefreshTokenResponseDto>(content, response);
        CheckForStateMismatch(request.State, refreshResult.State, response);

        return refreshResult.ToRefreshTokenResponse();

    }

    public async Task<bool> RevokeAccessToken(string accessToken, int clientId, CancellationToken ct)
    {
        var parameters = new[]
        {
            new KeyValuePair<string, string>("access_token", accessToken),
            new KeyValuePair<string, string>("client_id", clientId.ToString())
        };
        using var requestContent = new FormUrlEncodedContent(parameters);

        var response = await TryPostRequest(RevokeTokenEndpoint, requestContent, ct);
        
        var content = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            ThrowResponseError(content, response);
        }
        
        var revokeResult = TryDeserialize<VkSuccessResponseDto>(content, response);
        
        return revokeResult.Response == 1;
    }

    public async Task<bool> Logout(string accessToken, int clientId, CancellationToken ct)
    {
        var parameters = new[]
        {
            new KeyValuePair<string, string>("access_token", accessToken),
            new KeyValuePair<string, string>("client_id", clientId.ToString())
        };
        using var requestContent = new FormUrlEncodedContent(parameters);

        var response = await TryPostRequest(LogoutEndpoint, requestContent, ct);

        var content = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            ThrowResponseError(content, response);
        }

        var logoutResult = TryDeserialize<VkSuccessResponseDto>(content, response);

        return logoutResult.Response == 1;
    }

    public async Task<UserInfo> GetUserInfo(Guid userId, string accessToken, int clientId, CancellationToken ct)
    {
        var parameters = new[]
        {
            new KeyValuePair<string, string>("access_token", accessToken),
            new KeyValuePair<string, string>("client_id", clientId.ToString())
        };
        using var requestContent = new FormUrlEncodedContent(parameters);
        
        var response = await TryPostRequest(UserInfoEndpoint, requestContent, ct);
        var content = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
        {
            ThrowResponseError(content, response);
        }
        
        var userInfoResult = TryDeserialize<VkUserInfoDto>(content, response);

        return userInfoResult.ToUserInfo(userId);
    }
    
    private async Task<HttpResponseMessage> TryPostRequest(string endpoint, FormUrlEncodedContent content, CancellationToken ct)
    {
        HttpResponseMessage response;
        try
        {
            using var client = _httpClientFactory.CreateClient("vkid");
            response = await client.PostAsync(endpoint, content, ct);
        }
        catch (HttpRequestException e)
        {
            throw new ExternalApplicationException("Network error while connecting to VK ID",
                ExternalErrorCode.NetworkError,
                500,
                innerException: e);
        }

        return response;
    }
    
    private static T TryDeserialize<T>(string content, HttpResponseMessage response) where T : class
    {
        T? result;
        try
        {
            result = JsonSerializer.Deserialize<T>(content);
        }
        catch (Exception ex) when (ex is ArgumentNullException or JsonException)
        {
            throw new ExternalApplicationException("Failed to deserialize VK response",
                ExternalErrorCode.VkIdUnexpectedResponse,
                (int)response.StatusCode);
        }
        
        if (result is null)
        {
            throw new ExternalApplicationException("Failed to deserialize VK response",
                ExternalErrorCode.VkIdUnexpectedResponse,
                (int)response.StatusCode);
        }

        return result;
    }
    
    private static void CheckForStateMismatch(string sentState, string receivedState, HttpResponseMessage response)
    {
        if (receivedState != sentState)
        {
            throw new ExternalApplicationException("Request may be intercepted: state mismatch",
                ExternalErrorCode.VkIdStateMismatch,
                (int)response.StatusCode);
        }
    }
    
    private static void ThrowResponseError(string content, HttpResponseMessage response)
    {
        var errorResult = TryDeserialize<VkErrorResponseDto>(content, response);
        throw new ExternalApplicationException(
            errorResult?.ErrorDescription ?? "VK API request failed",
            errorResult?.Error.ToExternalErrorCode() ?? ExternalErrorCode.VkIdUnavailable,
            (int)response.StatusCode);
    }
}