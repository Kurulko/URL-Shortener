using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using URL_ShortenerAPI.Data;

namespace URL_ShortenerAPI.Controllers;

public class HomeController : Controller
{
    [Route("about")]
    public IActionResult About()
    {
        return View();
    }
}
