using System.Text.Json.Serialization;

namespace VkPhotosExtractor.Web.Models;

public class AuthCallbackPayload
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("device_id")]
    public string DeviceId { get; set; }

}