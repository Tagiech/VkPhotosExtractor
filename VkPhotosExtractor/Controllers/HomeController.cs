using Microsoft.AspNetCore.Mvc;

namespace VkPhotosExtractor.Controllers;

[Controller]
public class HomeController : Controller
{
    //[Authorize]
    [ActionName("Index")]
    public IActionResult Index()
    {
        return View("Index");
    }
}