using Server.Services;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Server.Util;

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
            return Unauthorized(new { message = "User not authenticated.", success = false });
        }
        bool result = await _cartService.AddToCart(item, Convert.ToString(account?.Id)!, address);

        if (result)
        {
            return Ok(new { success = true });
        }

        return BadRequest(new { message = "Failed to add item to cart." , success = false});
    }

    [HttpGet("GetCart")]
    public async Task<IActionResult> GetCartItems()
    {
        Account? user = AccountUtilities.GetAccount(HttpContext);
        if (user == null) return BadRequest(new { message = "user could not be found", success = false });

        Item[] cartItems = await _cartService.GetCartItems(user.Id.ToString());
        return Ok(new { success = true, cartItems });
    }

    [HttpGet("PurchaseCartItems")]
    public async Task<IActionResult> PurchaseCartItems()
    {
        Account? user = AccountUtilities.GetAccount(HttpContext);

        if (user == null) return BadRequest(new { message = "could not find user", success = false });

        bool success = await _cartService.PurchaseCartItems(user.Id.ToString());

        if (success) return Ok(new { success = true });

        return BadRequest(new { message = "Could not purchase all cart items", success = false });
    }

    [HttpDelete("RemoveFromCart/{itemId}")]
    public async Task<IActionResult> RemoveFromCart(string itemId)
    {
        Account? user = AccountUtilities.GetAccount(HttpContext);
        if (user == null)
        {
            return BadRequest(new { message = "could not find user", success = false });
        }

        bool success = await _cartService.RemoveFromCart(itemId, user.Id.ToString());

        if (success)
        {
            return Ok(new { success = true });

        }

        return BadRequest(new { message = "Could not remove item from cart", success = false });
    }
}