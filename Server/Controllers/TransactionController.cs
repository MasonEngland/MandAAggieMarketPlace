using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Server.Context;
using Server.Models;
using Server.Util;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("/Api/Transactions")]
public class TransactionController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly DatabaseContext _db;

    public TransactionController(TransactionService transactionService, DatabaseContext db)
    {
        _db = db;
        _transactionService = transactionService;
        
    }

    [HttpPost("Checkout")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] string[] itemIds)
    {
        Account? account = AccountUtilities.GetAccount(HttpContext);
        if (account == null)
        {
            return Unauthorized(new {success = false, message = "could not authenticate"});
        }

        Item[] dbItems = new Item[itemIds.Length];
        for (int i = 0; i < itemIds.Length; i++)
        {
            Item item = await _db.CurrentStock.Where(h => h.Id.ToString() == itemIds[i]).FirstAsync();
            dbItems[i] = item;
        }

        string? clientSecret = await _transactionService.CreateCheckoutSession(dbItems);

        if (clientSecret == null)
        {
            return StatusCode(500);
        }

        return Ok(new {success = true, clientSecret, dbItems});
    }

    [HttpGet("CheckoutStatus")]
    public IActionResult HandleCheckoutStatus()
    {
        return Ok();
    }

}