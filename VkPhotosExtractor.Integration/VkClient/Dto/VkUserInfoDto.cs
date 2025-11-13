using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace VkPhotosExtractor.Integration.VkClient.Dto;

[UsedImplicitly]
public class VkUserInfoDto
{
    [JsonPropertyName("user")]
    public VkUserDto User { get; set; } = null!;
}

[UsedImplicitly]
public class VkUserDto
{
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = "";
    
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = "";
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = "";
    
    [JsonPropertyName("avatar")]
    public string? PhotoUrl { get; set; }
    
    [JsonPropertyName("sex")]
    public int Sex { get; set; }
    
    [JsonPropertyName("verified")]
    public bool Verified { get; set; }
    
    [JsonPropertyName("birthday")]
    public string Birthday { get; set; } = "";
}