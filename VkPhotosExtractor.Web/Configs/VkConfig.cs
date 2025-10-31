namespace VkPhotosExtractor.Web.Configs;

public class VkConfig
{
    public int AppId { get; init; }
    public string PrivateKey { get; init; }

    public VkConfig(int appId, string privateKey)
    {
        AppId = appId;
        PrivateKey = privateKey;
    }
}