using Microsoft.AspNetCore.Mvc;

namespace VkPhotosExtractor.Web.Controllers.Models;

public class AuthCallbackRequest
{
    [FromQuery(Name = "code")]
    public string? Code { get; set; }
    
    [FromQuery(Name = "device_id")]
    public string? DeviceId { get; set; }
    
    [FromQuery(Name = "state")]
    public string? State { get; set; }
}