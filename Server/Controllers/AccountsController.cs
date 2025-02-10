using Server.Models;
using Server.Context;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;


[Route("Api/Accounts")]
public class AccountsController : Controller
{
    private DatabaseContext _db;

    public AccountsController(DatabaseContext db)
    {
        _db = db;
    }

    // just a help function to deserialize token data
    private Account? GetAccount()
    {
        string? data = Convert.ToString(HttpContext.Items["tokendata"]);
        if (data == null) 
        {
            return null; 
        }

        Account? account = JsonSerializer.Deserialize<Account>(data!);
        if (account == null)
        {
            return null;
        }
        return account; 
    }

    // Route: /Api/Accounts/Delete/id
    [HttpDelete("Delete/{id}")]
    public IActionResult DeleteAccount(String id) 
    {
        try
        {
            Account account = _db.accounts.Where(i => Convert.ToString(i.Id) == id).ToArray()[0];

            _db.accounts.Remove(account);

            return Ok(new {success = true});
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return StatusCode(500);
        }
    }

    // Route: /Api/Accounts/Balance/{funds}
    [HttpPut("Balance/{funds}")]
    public IActionResult AddFunds(double funds)
    {
        Account? account = GetAccount();
        if (account == null)
        {
            return Unauthorized(new { success = false });
        }

        if (funds < 0 && Math.Abs(funds) > account.Balance)
        {
            return BadRequest(new {success = false, message = "not enough funds"});
        }

        _db.accounts
            .Where(h => h.Id == account.Id)
            .ExecuteUpdate(setter => setter.SetProperty(b => b.Balance, b => b.Balance + funds));

        return Ok(new {success = true});
    }

    //Route: /Api/Accounts/Purchases
    [HttpGet("Purchases")]
    public IActionResult GetPurchases() 
    {
        Account? account = GetAccount();
        if (account == null) 
        {
            return Unauthorized(new {
                success = false,
                message = "Token could not be deserialized"
            });
        }

        var Items = _db.OrderQueue.Where(b => b.OwnerId == Convert.ToString(account.Id)).ToArray();

        if (Items.Length == 0) 
        {
            return Ok(new {
                success = true,
                messgae = "no items to be found"
            });
        }

        return Ok(new {
            success = true,
            content = Items,
        });
    }
}
