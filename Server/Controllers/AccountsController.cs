using Server.Models;
using Server.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Services;
using static Server.Util.AccountUtilities;


[ApiController]
[Route("Api/Accounts")]
public class AccountsController : Controller
{
    private DatabaseContext _db;
    private IAccountService _accountService;

    public AccountsController(DatabaseContext db, IAccountService accountService)
    {
        /*
         * this constructor is used to inject the database context and the account service into the controller
         * this allows us to use the database context and the account service in the controller methods
         * the account service will contain all the business logic and the controller endpoints will only validate input and return the result
         */
        _accountService = accountService;
        _db = db;
    }

    [HttpDelete("{id}")]
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

    [HttpPut("Balance/{funds}")]
    public async Task<IActionResult> AddFunds(double funds)
    {
        Account? account = GetAccount(HttpContext);
        if (account == null)
        {
            return Unauthorized(new { success = false });
        }

        if (funds < 0 && Math.Abs(funds) > account.Balance)
        {
            return BadRequest(new {success = false, message = "not enough funds"});
        }

        bool result = await _accountService.AddFunds(Convert.ToString(account.Id)!, funds);
        if (!result)
        {
            return NotFound(new { success = false });
        }
        

        return Ok(new { success = true });
    }

    [HttpGet("Purchases")]
    public async Task<IActionResult> GetPurchases() 
    {
        Account? account = GetAccount(HttpContext);
        if (account == null) 
        {
            return Unauthorized(new {
                success = false,
                message = "Token could not be deserialized"
            });
        }

        Order[]? Items = await _accountService.GetPurchases(Convert.ToString(account.Id)!);
        if (Items == null) 
        {
            return NotFound(new {
                success = false,
                message = "No purchases found"
            });
        }

        return Ok(new {
            success = true,
            content = Items,
        });
    }

    [HttpGet("GetAccount")]
    public async Task<IActionResult> GetOneAccount()
    {
        Account? accountFromToken = GetAccount(HttpContext);
        if (accountFromToken == null)
        {
            return Unauthorized(new { success = false });
        }

        Account? account = await _accountService.GetAccount(Convert.ToString(accountFromToken.Id)!);
        if (account == null)
        {
            return NotFound(new { success = false });
        }


        return Ok(new { success = true, account = account });
    }

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateAccount([FromBody] Account account)
    {
        Account? currentAccount = GetAccount(HttpContext);
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

        Account? updatedAccount = await _accountService.UpdateAccount(account);
        if (updatedAccount == null)
        {
            return NotFound(new { success = false });
        }

        string secret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")!;
        string token = MakeToken(updatedAccount)!;
        
        return Ok(new { success = true, account, token });
    }

    [HttpPut("UpdatePassword")]
    public async Task<IActionResult> UpdatePassword([FromBody] PasswordParams passwordParams)
    {
        Account? currentAccount = GetAccount(HttpContext);
        if (currentAccount == null)
        {
            return Unauthorized(new { success = false });
        }

        if (Convert.ToString(currentAccount.Id) != passwordParams.AccountId)
        {
            return Unauthorized(new { success = false });
        }

        bool result = await _accountService.ChangePassword(Convert.ToString(passwordParams.AccountId)!, passwordParams.OldPassword, passwordParams.NewPassword);

        if (!result)
        {
            return BadRequest(new { success = false });
        }

        return Ok(new { success = true });
    }
}
