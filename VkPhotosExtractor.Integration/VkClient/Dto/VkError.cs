using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace VkPhotosExtractor.Integration.VkClient.Dto;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VkError
{
    [EnumMember(Value = "access_denied")]
    AccessDenied,
    
    [EnumMember(Value = "invalid_token")]
    InvalidToken,
    
    [EnumMember(Value = "server_error")]
    ServerError,
    
    [EnumMember(Value = "slow_down")]
    SlowDown,
    
    [EnumMember(Value = "temporarily_unavailable")]
    TemporarilyUnavailable,
    
    [EnumMember(Value = "invalid_client")]
    InvalidClient
}