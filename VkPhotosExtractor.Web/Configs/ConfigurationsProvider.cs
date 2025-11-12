using Microsoft.Extensions.Options;
using VkPhotosExtractor.Application.Configurations;
// ReSharper disable ConvertIfStatementToReturnStatement

namespace VkPhotosExtractor.Web.Configs;

public class ConfigurationsProvider : IConfigurationsProvider
{
    private readonly IOptions<VkConfig> _vkConfig;
    private readonly IOptions<JwtConfig> _jwtConfig;
    private readonly IOptions<HostsConfig> _hostsConfig;

    public ConfigurationsProvider(IOptions<VkConfig> vkConfig,
        IOptions<JwtConfig> jwtConfig,
        IOptions<HostsConfig> hostsConfig)
    {
        _vkConfig = vkConfig;
        _jwtConfig = jwtConfig;
        _hostsConfig = hostsConfig;
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
        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new ArgumentNullException(nameof(jwtKey), "JWT Key is not configured properly.");
        }

        return jwtKey;
    }

    public string GetJwtIssuer()
    {
        var jwtIssuer = _jwtConfig.Value.Issuer;
        if (string.IsNullOrWhiteSpace(jwtIssuer))
        {
            throw new ArgumentNullException(nameof(jwtIssuer), "JWT Issuer is not configured properly.");
        }

        return jwtIssuer;
    }

    public string GetJwtAudience()
    {
        var jwtAudience = _jwtConfig.Value.Audience;
        if (string.IsNullOrWhiteSpace(jwtAudience))
        {
            throw new ArgumentNullException(nameof(jwtAudience), "JWT Audience is not configured properly.");
        }
        
        return jwtAudience;
    }
    
    public string GetBackendHost()
    {
        var backendHost = _hostsConfig.Value.Backend;
        if (string.IsNullOrWhiteSpace(backendHost))
        {
            throw new ArgumentNullException(nameof(backendHost), "Backend Host is not configured properly.");
        }
        
        return RemoveTrailingSlash(backendHost);
    }

    public string GetFrontendHost()
    {
        var frontendHost = _hostsConfig.Value.Frontend;
        if (string.IsNullOrWhiteSpace(frontendHost))
        {
            throw new ArgumentNullException(nameof(frontendHost), "Frontend Host is not configured properly.");
        }
        
        return RemoveTrailingSlash(frontendHost);
    }
    
    public string GetVkIdHost()
    {
        var vkIdHost = _hostsConfig.Value.VkId;
        if (string.IsNullOrWhiteSpace(vkIdHost))
        {
            throw new ArgumentNullException(nameof(vkIdHost), "VK ID Host is not configured properly.");
        }

        return RemoveTrailingSlash(vkIdHost);
    }

    private static string RemoveTrailingSlash(string url)
    {
        if (url[^1] == '/')
        {
            //TODO: add warning log here about removing trailing slash from config
            return url[..^1];
        }

        return url;
    }
}