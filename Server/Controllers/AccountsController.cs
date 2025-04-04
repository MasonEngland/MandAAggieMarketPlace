using Server.Models;
using Server.Context;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using JWT.Builder;
using JWT.Algorithms;
using Microsoft.Net.Http.Headers;


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
        string? data = Convert.ToString(HttpContext.Items["tokenData"]);
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

    [HttpGet("GetAccount")]
    public IActionResult GetOneAccount()
    {
        Account? account = GetAccount();
        if (account == null)
        {
            return Unauthorized(new { success = false });
        }


        return Ok(new { success = true, account = account });
    }

    // Route: /Api/Accounts/Update
    [HttpPut("Update")]
    public IActionResult UpdateAccount([FromBody] Account account)
    {
        Account? currentAccount = GetAccount();
        if (currentAccount == null)
        {
            return Unauthorized(new { success = false });
        }

        if (currentAccount.Id != account.Id)
        {
            return Unauthorized(new { success = false });
        }

        if (account.FirstName == "" || account.LastName == "" || account.Email == "") {
            return BadRequest(new { success = false, message = "field missing" });
        }

        _db.accounts
            .Where(h => h.Id == account.Id)
            .ExecuteUpdate(setter => setter
                .SetProperty(b => b.FirstName, account.FirstName)
                .SetProperty(b => b.LastName, account.LastName)
                .SetProperty(b => b.Email, account.Email)
            );
        
        Account[] gotAccount = _db.accounts.Where(h => h.Id == account.Id).ToArray();

        string secret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")!;
        string token = JwtBuilder
            .Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(secret)
            .AddClaim("Id", gotAccount[0].Id)
            .AddClaim("Email", gotAccount[0].Email)
            .AddClaim("Password", gotAccount[0].Password)
            .AddClaim("FirstName", gotAccount[0].FirstName)
            .AddClaim("LastName", gotAccount[0].LastName)
            .AddClaim("Admin", gotAccount[0].Admin)
            .Encode();
        
        
        // TODO: find a way to set the Set-Cookie header in response 
        var cookie = new SetCookieHeaderValue("testCookie", "myValue");
        cookie.Expires = DateTimeOffset.Now.AddDays(1);
        Request.Headers.Append("Set-Cookie", cookie.ToString());
        return Ok(new { success = true, account, token });
    }

    // Route: /Api/Accounts/UpdatePassword
    [HttpPut("UpdatePassword")]
    public IActionResult UpdatePassword([FromBody] Account account)
    {
        Account? currentAccount = GetAccount();
        if (currentAccount == null)
        {
            return Unauthorized(new { success = false });
        }

        if (currentAccount.Id != account.Id)
        {
            return Unauthorized(new { success = false });
        }

        try
        {
            _db.accounts
            .Where(h => h.Id == account.Id)
            .ExecuteUpdate(setter => setter
                .SetProperty(b => b.Password, account.Password)
            );
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return StatusCode(500);
        }

        return Ok(new { success = true });
    }
}
