using System.Text.Json.Serialization;

namespace VkPhotosExtractor.Integration.VkClient.Dto;

public class VkErrorResponseDto
{
    [JsonPropertyName("error")]
    public VkError Error { get; set; }
    
    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; set; }
}