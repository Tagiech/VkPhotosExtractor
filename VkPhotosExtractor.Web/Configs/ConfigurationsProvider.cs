using Microsoft.Extensions.Options;
using VkPhotosExtractor.Application.Configurations;

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

    public int GetVkAppId() => _vkConfig.Value.AppId;
    
    public string GetJwtKey() => _jwtConfig.Value.Key;
    public string GetJwtIssuer() => _jwtConfig.Value.Issuer;
    public string GetJwtAudience() => _jwtConfig.Value.Audience;
    
}