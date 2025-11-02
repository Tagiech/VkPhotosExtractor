using System.Text.Json.Serialization;

namespace VkPhotosExtractor.Integration.VkClient.Dto;

public class VkSuccessResponseDto
{
    [JsonPropertyName("response")]
    public int Response { get; set; }
}