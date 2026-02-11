using Server.Services;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Server.Util;
using Server.Context;

[ApiController]
[Route("/Api/Cart")]
public class CartController : Controller
{
    private readonly IShoppingCartService _cartService;
    private readonly DatabaseContext _db;
    public CartController(IShoppingCartService cartService, DatabaseContext db)
    {
        _cartService = cartService;
        _db = db;

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

        try
        {
            var transaction = await _db.Database.BeginTransactionAsync();
            CartItem[] cartItems = await _cartService.GetCartItems(user.Id.ToString());
            Item[] items = _db.CurrentStock
                .Where(i => cartItems.Select(ci => ci.OrderItemId).Contains(i.Id))
                .ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                items[i].Stock = cartItems.First(ci => ci.OrderItemId == items[i].Id).Amount;
            }

            

            await transaction.RollbackAsync();
            return Ok(new { success = true, cartItems, items });

            
        } catch (Exception ex)
        {
            Console.WriteLine("Error in GetCartItems: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
            return StatusCode(500, new { message = "internal server error", success = false });
        }

        
    }

    [HttpGet("PurchaseCartItems")]
    public async Task<IActionResult> PurchaseCartItems()
    {
        try
        {
            Account? user = AccountUtilities.GetAccount(HttpContext);

            if (user == null) return BadRequest(new { message = "could not find user", success = false });

            bool success = await _cartService.PurchaseCartItems(user.Id.ToString());

            if (success) return Ok(new { success = true });

            return BadRequest(new { message = "Could not purchase all cart items", success = false });
        } catch (Exception ex)
        {
            Console.WriteLine("Error in PurchaseCartItems: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
            return StatusCode(500, new { message = "internal server error", success = false });
        }
        
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