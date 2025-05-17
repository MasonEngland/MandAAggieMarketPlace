using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Server.Models;
using JWT.Builder;
using JWT.Algorithms;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;
using static Server.Util.AccountUtilities;

namespace Server.Controllers;

[ApiController]
[Route("Api/Auth")]
public class AuthController : Controller
{
    private readonly DatabaseContext _db;
    public AuthController(DatabaseContext db) 
    {
        _db = db;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] Account account)
    {
        string secret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")!; 


        if (account.Email == null || 
            account.Password == null || 
            account.FirstName == null || 
            account.LastName == null
        ) 
        {
            return BadRequest("body of request is invalid");
        }

        try 
        {
            Account[] takenmail = _db.accounts
                .Where(item => item.Email == account.Email).ToArray();

            if (takenmail.Length >= 1) 
            {
                return BadRequest("Email already taken");
            }

            Account createdAccount = new Account() {
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                Password = HashPassword(account.Password),
                Balance = 0.0f,
                Admin = false
            };
            await _db.accounts.AddAsync(createdAccount);
            await _db.SaveChangesAsync();

            var token = MakeToken(createdAccount);


            return Created($"/Api/Accounts/{createdAccount.Id}", new 
            {
                Success = true,
                Id = createdAccount.Id,
                Email = createdAccount.Email,
                FirstName = createdAccount.FirstName,
                LastName = createdAccount.LastName,
                Token = token
            });
            
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] Account account) 
    {
        /**
        * requires a full acount object be passed in the request body
        * required fields are Email, Password
        * will return a JWT as well as the account information if authentication is successful
        */
        string secret = Environment.GetEnvironmentVariable("ACCESS_TOKEN_SECRET")!; 
        
        try 
        {
            if (account.Email == null || account.Password == null) 
            {
                return BadRequest("body of request is invalid");
            }

            Account[] gotAccount = await _db.accounts
                .Where(item => item.Email.ToLower() == account.Email.ToLower())
                .ToArrayAsync();

            if (gotAccount.Length < 1) 
            {
                return NotFound("no account with matching email");
            }

            if (!Verify(account.Password, gotAccount[0].Password))
            {
                return Unauthorized("Password is incorrect");
            }

            var token = MakeToken(gotAccount[0]);

            return Ok(new
            {
                Success = true,
                Id = gotAccount[0].Id,
                Email = gotAccount[0].Email,
                FirstName = gotAccount[0].FirstName,
                LastName = gotAccount[0].LastName,
                Token = token
            });
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return StatusCode(500);
        }
    }
}
