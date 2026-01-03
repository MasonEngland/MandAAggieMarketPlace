using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Server.Context;
using Server.Models;
using Server.Util;

namespace Server.Controllers;

[ApiController]
[Route("/Api/Transactions")]
public class TransactionController : Controller
{
    private readonly ITransactionService _transactionService;

    public TransactionController(TransactionService transactionService)
    {
        _transactionService = transactionService;
        
    }

    [HttpPost("Checkout")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] Item[] items)
    {
        Account? account = AccountUtilities.GetAccount(HttpContext);
        if (account == null)
        {
            return Unauthorized(new {success = false, message = "could not authenticate"});
        }

        string? clientSecret = await _transactionService.CreateCheckoutSession(items);

        if (clientSecret == null)
        {
            return StatusCode(500, new {success = false, message = "could not create checkout session"});
        }

        return Ok(new {success = true, clientSecret, items});
    }

    [HttpPost("CheckoutStatus")]
    public async Task<IActionResult> HandleCheckoutStatus([FromBody] CheckoutStatusRequest request)
    {
        Account? account = AccountUtilities.GetAccount(HttpContext);
        if (account is null)
        {
            return Unauthorized(new {success = false, message = "could not find account"});
        }

        bool success = await _transactionService.HandleCheckoutStatus(request.SessionId, account.Id.ToString(), request.Items, request.Address);
        if (!success)
        {
            return StatusCode(500, new { success = false, message = "could not process checkout status" });
        }
        return Ok(new { success = true });
    }
}