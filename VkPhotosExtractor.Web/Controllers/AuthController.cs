using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
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
    private readonly ISecurityStringProvider _securityStringProvider;
    
    public AuthController(IAuthService authService,
        IConfigurationsProvider configurationsProvider,
        ISecurityStringProvider securityStringProvider)
    {
        _authService = authService;
        _configurationsProvider = configurationsProvider;
        _securityStringProvider = securityStringProvider;
    }

    [HttpGet("params")]
    public IActionResult GetAuthParams()
    {
        var vkAuthParams = _authService.GetVkAuthParams();

        var viewModel = new AuthParamsViewModel
        {
            VkAppId = vkAuthParams.VkAppId,
            RedirectUrl = vkAuthParams.FrontendRedirectUrl,
            State = vkAuthParams.State,
            CodeChallenge = vkAuthParams.CodeChallenge,
            AuthRequestUri = vkAuthParams.AuthRequestUri
        };

        return Ok(viewModel);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> AuthCallback([FromQuery] AuthCallbackRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code) 
            || string.IsNullOrWhiteSpace(request.State) 
            || string.IsNullOrWhiteSpace(request.DeviceId))
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
        var userClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userClaim) || !Guid.TryParse(userClaim, out var userId))
        {
            return Unauthorized("Invalid user ID in token");
        }

        await _authService.Logout(userId, ct);
        await HttpContext.SignOutAsync();

        return Ok();
    }

    [Authorize]
    [HttpGet("userinfo")]
    public async Task<IActionResult> GetUserInfo(CancellationToken ct = default)
    {
        var userClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userClaim) || !Guid.TryParse(userClaim, out var userId))
        {
            return Unauthorized("Invalid user ID in token");
        }
        
        var userInfo = await _authService.GetUserInfo(userId, ct);
        var viewModel = new UserInfoViewModel
        {
            FirstName = userInfo.FirstName,
            LastName = userInfo.LastName,
            PhotoUrl = userInfo.PhotoUrl!
        };
        
        return Ok(viewModel);
    }
    
    private void CreateJwtToken(string userId, DateTime expiresAt)
    {
        var utcNowOffset = DateTimeOffset.UtcNow;
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, userId),
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
        
        var csrfToken = _securityStringProvider.GenerateRandomString(32);
        Response.Cookies.Append("CSRF-Token", csrfToken, new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiresAt
        });
    }
}