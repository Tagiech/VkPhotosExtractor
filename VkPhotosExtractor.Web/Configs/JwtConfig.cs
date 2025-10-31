namespace VkPhotosExtractor.Web.Configs;

public class JwtConfig
{
    public string Key { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
}