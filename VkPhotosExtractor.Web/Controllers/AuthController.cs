using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VkPhotosExtractor.Application.Auth;
using VkPhotosExtractor.Application.Configurations;

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

    //https://id.vk.ru/authorize?
    //response_type=code&
    //client_id=12345&
    //scope=email%20phone&
    //redirect_uri=https%3A%2F%2Fyour.site&
    //state=XXXRandomZZZ&
    //code_challenge=K8KAyQ82WSEncryptedVerifierGYUDj8K&
    //code_challenge_method=S256
    [HttpGet("params")]
    public IActionResult GetAuthUri()
    {
        var redirectUrl = Url.Action("AuthCallback", "Auth", null, Request.Scheme, Request.Host.ToString());
        if (redirectUrl is null)
        {
            return StatusCode(500, "Failed to generate redirect URL");
        }
        var authQueryParams = _authService.GetVkAuthQueryParams(redirectUrl);

        return Ok(authQueryParams);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> AuthCallback([FromQuery] string code,
        [FromQuery(Name = "device_id")] string deviceId,
        [FromQuery] string state)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state) || string.IsNullOrEmpty(deviceId))
        {
            return BadRequest("Missing required query parameters");
        }

        var authProcessExists = _authService.CheckIfAuthProcessExists(state);
        if (!authProcessExists)
        {
            return BadRequest("Invalid or expired state");
        }
        
        var redirectUrl = Url.Action("AuthCallback", "Auth", null, Request.Scheme, Request.Host.ToString());
        if (redirectUrl is null)
        {
            return StatusCode(500, "Failed to generate redirect URL");
        }

        var vkAuthResponse = await _authService.ObtainAccessToken(state, code, deviceId, redirectUrl);
        if (vkAuthResponse is null)
        {
            return StatusCode(500, "Failed to obtain access token");
        }
        
        
        /// Создаём наш JWT (внутренний токен)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, vkAuthResponse.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)

        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationsProvider.GetJwtKey()));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddSeconds(vkAuthResponse.ExpiresIn);

        var token = new JwtSecurityToken(
            issuer: _configurationsProvider.GetJwtIssuer(),
            audience: _configurationsProvider.GetJwtAudience(),
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Сохраняем токен в HttpOnly cookie
        Response.Cookies.Append("jwt", tokenString, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expires
        });

        return Ok(new { token = tokenString });
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
}