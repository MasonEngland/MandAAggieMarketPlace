using Server.Services;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Server.Util;
using System.Xml.Schema;

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
        Account? account = AccountUtilities.GetAccount(HttpContext);
        if (account == null)
        {
            return Unauthorized("User not authenticated.");
        }
        bool result = await _cartService.AddToCart(item, Convert.ToString(account?.Id)!, address);

        if (result)
        {
            return Ok("Item added to cart successfully.");
        }

        return BadRequest("Failed to add item to cart.");
    }

    [HttpGet("GetCart")]
    public async Task<IActionResult> GetCartItems()
    {
        Account? user = AccountUtilities.GetAccount(HttpContext);
        if (user == null) return BadRequest("user could not be found");

        Item[] cartItems = await _cartService.GetCartItems(user.Id.ToString());
        return Ok(cartItems);
    }
}