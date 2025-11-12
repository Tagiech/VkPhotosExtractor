using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace VkPhotosExtractor.Integration.VkClient.Dto;

[UsedImplicitly]
public class VkErrorResponseDto
{
    [JsonPropertyName("error")]
    public VkError Error { get; set; }

    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; set; } = "";
}