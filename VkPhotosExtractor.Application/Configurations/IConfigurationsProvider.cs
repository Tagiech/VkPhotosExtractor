namespace VkPhotosExtractor.Application.Configurations;

public interface IConfigurationsProvider
{
    int GetVkAppId();
    string GetJwtKey();
    string GetJwtIssuer();
    string GetJwtAudience();
}