namespace VkPhotosExtractor.Web.Controllers.Models.ViewModels;

public class AuthParamsViewModel
{
    public int VkAppId { get; set; }
    public string ReturnUri { get; set; } = "";
    public string State { get; set; } = "";
    public string CodeChallenge { get; set; } = "";
    public string AuthRequestUri { get; set; } = "";
}