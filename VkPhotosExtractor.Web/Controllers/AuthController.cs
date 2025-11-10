using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VkPhotosExtractor.Application.Auth;
using VkPhotosExtractor.Application.Configurations;
using VkPhotosExtractor.Web.Controllers.Models;
using VkPhotosExtractor.Web.Controllers.Models.ViewModels;

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
    public IActionResult GetAuthParams()
    {
        var redirectUrl = Url.Action("AuthCallback", "Auth", null, Request.Scheme, Request.Host.ToString());
        if (redirectUrl is null)
        {
            return StatusCode(500, "Failed to generate redirect URL");
        }
        var vkAuthParams = _authService.GetVkAuthParams(redirectUrl);

        var viewModel = new AuthParamsViewModel
        {
            VkAppId = vkAuthParams.VkAppId,
            ReturnUri = vkAuthParams.ReturnUri,
            State = vkAuthParams.State,
            CodeChallenge = vkAuthParams.CodeChallenge,
            AuthRequestUri = vkAuthParams.AuthRequestUri
        };

        return Ok(viewModel);
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

        var userWithTokenExpiration = await _authService.ObtainAccessToken(request.State,
            request.Code,
            request.DeviceId,
            redirectUrl,
            ct);
        
        if (userWithTokenExpiration is null)
        {
            return Unauthorized("Something went wrong during VK authentication");
        }
        var (userId, tokenExpiresAt) = userWithTokenExpiration.Value;

        CreateJwtToken(userId.ToString(), tokenExpiresAt);

        return Ok();
    }

    [Authorize]
    [HttpGet("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct = default)
    {
        var userClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userClaim) || !Guid.TryParse(userClaim, out var userId))
        {
            return Unauthorized("Invalid user ID in token");
        }

        await _authService.Logout(userId, ct);

        return Ok();
    }
    
    private void CreateJwtToken(string userId, DateTime expiresAt)
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

        var token = new JwtSecurityToken(
            issuer: _configurationsProvider.GetJwtIssuer(),
            audience: _configurationsProvider.GetJwtAudience(),
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        Response.Cookies.Append("jwt", tokenString, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt
        });
    }
}