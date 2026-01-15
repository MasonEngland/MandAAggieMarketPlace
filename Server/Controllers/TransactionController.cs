using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Util;

namespace Server.Controllers;

[ApiController]
[Route("/Api/Transactions")]
public class TransactionController : Controller
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
        
    }

    [HttpPost("Checkout/{address}")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] Item item, string address)
    {
        Account? account = AccountUtilities.GetAccount(HttpContext);
        if (account == null)
        {
            return Unauthorized(new {success = false, message = "could not authenticate"});
        }

        string? sessionId = await _transactionService.CreateCheckoutSession(item, address);

        if (sessionId == null)
        {
            return StatusCode(500, new {success = false, message = "could not create checkout session"});
        }

        return Ok(new {success = true, sessionId, item});
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