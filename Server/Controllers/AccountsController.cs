using Server.Models;
using Server.Context;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using JWT.Builder;
using JWT.Algorithms;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;


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
        //
        // gets the token data from the http contex and deserializes it 
        // returns null or the account object depending on the success of the deserialization
        //
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
    public async Task<IActionResult> DeleteAccount(String id) 
    {
        try
        {
            // get the account from the database by converting to an array and then getting the first element
            Account account = (await _db.accounts.Where(i => Convert.ToString(i.Id) == id).ToArrayAsync())[0];

            _db.accounts.Remove(account);
            await _db.SaveChangesAsync();

            return Ok(new {success = true});
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return StatusCode(500);
        }
    }

    // Route: /Api/Accounts/Balance/{funds}
    [HttpPut("Balance/{funds}")]
    public async Task<IActionResult> AddFunds(double funds)
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

        await _db.accounts
            .Where(h => h.Id == account.Id)
            .ExecuteUpdateAsync(setter => setter.SetProperty(b => b.Balance, b => b.Balance + funds));

        return Ok(new {success = true});
    }

    //Route: /Api/Accounts/Purchases
    [HttpGet("Purchases")]
    public async Task<IActionResult> GetPurchases() 
    {
        Account? account = GetAccount();
        if (account == null) 
        {
            return Unauthorized(new {
                success = false,
                message = "Token could not be deserialized"
            });
        }

        Order[] Items = await  _db.OrderQueue.Where(b => b.OwnerId == Convert.ToString(account.Id)).ToArrayAsync();

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
    public async Task<IActionResult> UpdateAccount([FromBody] Account account)
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

        await _db.accounts
            .Where(h => h.Id == account.Id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(b => b.FirstName, account.FirstName)
                .SetProperty(b => b.LastName, account.LastName)
                .SetProperty(b => b.Email, account.Email)
            );
        
        Account[] gotAccount = await _db.accounts.Where(h => h.Id == account.Id).ToArrayAsync();

        string secret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")!;
        string token = JwtBuilder
            .Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(secret)
            .AddClaim("Id", gotAccount[0].Id)
            .AddClaim("Email", gotAccount[0].Email)
            .AddClaim("FirstName", gotAccount[0].FirstName)
            .AddClaim("LastName", gotAccount[0].LastName)
            .AddClaim("Admin", gotAccount[0].Admin)
            .Encode();
        
        
        return Ok(new { success = true, account, token });
    }

    // Route: /Api/Accounts/UpdatePassword
    [HttpPut("UpdatePassword")]
    public async Task<IActionResult> UpdatePassword([FromBody] Account account)
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
            await _db.accounts
            .Where(h => h.Id == account.Id)
            .ExecuteUpdateAsync(setter => setter
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
