using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Util;
using Server.Context;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("/Api/Transactions")]
public class TransactionController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly DatabaseContext _db;

    public TransactionController(ITransactionService transactionService, DatabaseContext db)
    {
        _transactionService = transactionService;
        _db = db;
    }

    [HttpPost("Checkout/{address}")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CartItem[] items, string address)
    {
        Account? account = AccountUtilities.GetAccount(HttpContext);
        if (account == null)
        {
            return Unauthorized(new {success = false, message = "could not authenticate"});
        }

        string applicationUrl = $"{Request.Scheme}://{Request.Host}";

        string? sessionId = await _transactionService.CreateCheckoutSession(items, address, applicationUrl);

        if (sessionId == null)
        {
            return StatusCode(500, new {success = false, message = "could not create checkout session"});
        }

        

        return Ok(new {success = true, sessionId, items});
    }

    [HttpPost("CheckoutStatus")]
    public async Task<IActionResult> HandleCheckoutStatus([FromBody] CheckoutStatusRequest request)
    {
        Account? account = AccountUtilities.GetAccount(HttpContext);
        if (account is null)
        {
            Console.WriteLine("Could not find account in HandleCheckoutStatus");
            return Unauthorized(new {success = false, message = "could not find account"});
        }

        bool success = await _transactionService.HandleCheckoutStatus(request.SessionId, account.Id.ToString(), request.Address);
        if (!success)
        {
            return StatusCode(400, new { success = false, message = "checkout status was not successful" });
        }
        
        CartItem[] cartItems = await _db.CartItems.Where(c => c.OwnerId == account.Id.ToString()).ToArrayAsync();
         
        // remove cart itmes from user cart 
        foreach (CartItem item in cartItems)
        {
            _db.CartItems.Remove(item);
        }
        await _db.SaveChangesAsync();
        return Ok(new { success = true });
    }

    [HttpGet("GetSecret/{sessionId}")]
    public async Task<IActionResult> GetSecret(string sessionId)
    {
        Account? account = AccountUtilities.GetAccount(HttpContext);
        if (account is null)
        {
            return Unauthorized(new { success = false, message = "could not find account" });
        }

        string? secret = await _transactionService.GetSecret(sessionId);
        if (secret == null)
        {
            return StatusCode(500, new { success = false, message = "could not retrieve secret" });
        }

        return Ok(new { success = true, secret });
    }
}