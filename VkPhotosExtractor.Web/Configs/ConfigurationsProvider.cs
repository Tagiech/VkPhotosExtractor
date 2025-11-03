using Microsoft.Extensions.Options;
using VkPhotosExtractor.Application.Configurations;
// ReSharper disable ConvertIfStatementToReturnStatement

namespace VkPhotosExtractor.Web.Configs;

public class ConfigurationsProvider : IConfigurationsProvider
{
    private readonly IOptions<VkConfig> _vkConfig;
    private readonly IOptions<JwtConfig> _jwtConfig;

    public ConfigurationsProvider(IOptions<VkConfig> vkConfig, IOptions<JwtConfig> jwtConfig)
    {
        _vkConfig = vkConfig;
        _jwtConfig = jwtConfig;
    }

    public int GetVkAppId()
    {
        var vkAppId = _vkConfig.Value.AppId;
        if (vkAppId is null or <= 0)
        {
            throw new ArgumentNullException(nameof(vkAppId), "VK App ID is not configured properly.");
        }

        return vkAppId.Value;
    }
    public string GetJwtKey()
    {
        var jwtKey =  _jwtConfig.Value.Key;
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new ArgumentNullException(nameof(jwtKey), "JWT Key is not configured properly.");
        }

        return jwtKey;
    }

    public string GetJwtIssuer()
    {
        var jwtIssuer = _jwtConfig.Value.Issuer;
        if (string.IsNullOrEmpty(jwtIssuer))
        {
            throw new ArgumentNullException(nameof(jwtIssuer), "JWT Issuer is not configured properly.");
        }

        return jwtIssuer;
    }

    public string GetJwtAudience()
    {
        var jwtAudience = _jwtConfig.Value.Audience;
        if (string.IsNullOrEmpty(jwtAudience))
        {
            throw new ArgumentNullException(nameof(jwtAudience), "JWT Audience is not configured properly.");
        }
        
        return jwtAudience;
    }
}