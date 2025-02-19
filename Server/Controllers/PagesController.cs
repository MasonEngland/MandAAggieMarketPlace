using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/")]
public class PagesController : Controller 
{
    /**
     *
     *
     * this controller should handle the page routing
     * most likely won't be a super complicated class but 
     * will help keep the server architexture a little more clean
     *
     */
    [HttpGet]
    public IActionResult Home()
    {
        return File("index.html", "text/html");
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return File("index.html", "text/html");
    }
}
