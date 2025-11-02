using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VkPhotosExtractor.Application.Auth;
using VkPhotosExtractor.Application.Configurations;
using VkPhotosExtractor.Web.Controllers.Models;

namespace VkPhotosExtractor.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfigurationsProvider _configurationsProvider;
    
    public AuthController(IAuthService authService, IConfigurationsProvider configurationsProvider)
    {
        _authService = authService;
        _configurationsProvider = configurationsProvider;
    }

    [HttpGet("params")]
    public IActionResult GetAuthUri()
    {
        var redirectUrl = Url.Action("AuthCallback", "Auth", null, Request.Scheme, Request.Host.ToString());
        if (redirectUrl is null)
        {
            return StatusCode(500, "Failed to generate redirect URL");
        }
        var vkAuthRequest = _authService.CreateVkAuthRequest(redirectUrl);

        return Ok(vkAuthRequest);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> AuthCallback([FromQuery] AuthCallbackRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(request.Code) 
            || string.IsNullOrEmpty(request.State) 
            || string.IsNullOrEmpty(request.DeviceId))
        {
            return BadRequest("Missing required query parameters");
        }
        
        var redirectUrl = Url.Action("AuthCallback", 
            "Auth",
            null, 
            Request.Scheme,
            Request.Host.ToString());
        if (redirectUrl is null)
        {
            return StatusCode(500, "Failed to generate redirect URL");
        }

        var vkAuthResponse = await _authService.ObtainAccessToken(request.State,
            request.Code,
            request.DeviceId,
            redirectUrl,
            ct);

        CreateJwtToken(vkAuthResponse.UserId.ToString(), vkAuthResponse.ExpiresIn);

        return Ok();
    }

    // Пример защищённого эндпоинта
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var deviceId = User.FindFirstValue("device_id");
        return Ok(new { user_id = userId, device_id = deviceId });
    }
    
    private void CreateJwtToken(string userId, TimeSpan expiresIn)
    {
        var utcNowOffset = DateTimeOffset.UtcNow;
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, utcNowOffset.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        ];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationsProvider.GetJwtKey()));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenExpirationAt = DateTime.UtcNow.Add(expiresIn);

        var token = new JwtSecurityToken(
            issuer: _configurationsProvider.GetJwtIssuer(),
            audience: _configurationsProvider.GetJwtAudience(),
            claims: claims,
            expires: tokenExpirationAt,
            signingCredentials: signingCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        Response.Cookies.Append("jwt", tokenString, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = tokenExpirationAt
        });
    }
}