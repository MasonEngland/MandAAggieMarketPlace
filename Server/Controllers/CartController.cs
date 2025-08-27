using Server.Services;
using Server.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/Api/Cart")]
public class CartController : Controller
{
    private readonly IShoppingCartService _cartService;
    public CartController(IShoppingCartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("AddToCart/{address}")]
    public async Task<IActionResult> AddToCart([FromBody] Item item, string address)
    {
        var account = Server.Util.AccountUtilities.GetAccount(HttpContext);
        if (account == null)
        {
            return Unauthorized("User not authenticated.");
        }
        bool result = await _cartService.AddToCart(item, Convert.ToString(account?.Id)!, address);
        if (result)
            return Ok("Item added to cart successfully.");
        return BadRequest("Failed to add item to cart.");
    }
}