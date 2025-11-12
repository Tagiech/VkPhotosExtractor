using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace VkPhotosExtractor.Integration.VkClient.Dto;

[UsedImplicitly]
public class VkSuccessResponseDto
{
    [JsonPropertyName("response")]
    public int Response { get; set; }
}