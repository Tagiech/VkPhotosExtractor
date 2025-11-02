using System.Text.Json.Serialization;

namespace VkPhotosExtractor.Integration.VkClient.Dto;

public class VkAuthResponseDto
{
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
    
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("id_token")]
    public string IdToken { get; set; }
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    
    [JsonPropertyName("state")]
    public string State { get; set; }
    
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
}