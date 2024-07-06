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

}
