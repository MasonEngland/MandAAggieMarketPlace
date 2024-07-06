using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/")]
public class PagesController : Controller 
{
    [HttpGet]
    public IActionResult Home()
    {
        return File("index.html", "text/html");
    }
}
