using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Server.Models;
using static BCrypt.Net.BCrypt;
using JWT.Builder;
using JWT.Algorithms;

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
    public object Create([FromBody] Account account)
    {
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

            _db.accounts.Add(new Account() {
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                Password = HashPassword(account.Password),
                Balance = 0.0f,
                Admin = false
            });
            _db.SaveChanges();

            return StatusCode(201);
            
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("Login")]
    public object Login([FromBody] Account account) 
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

            Account[] gotAccount = _db.accounts
                .Where(item => item.Email.ToLower() == account.Email.ToLower())
                .ToArray();

            if (gotAccount.Length < 1) 
            {
                return NotFound("no account with matching email");
            }

            if (!Verify(account.Password, gotAccount[0].Password))
            {
                System.Console.WriteLine(account.Password);
                return Unauthorized("Password is incorrect");
            }

            // TODO: make a secure way to obtain admin privilage

            var token = JwtBuilder
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

            return new 
            {
                Success = true,
                Id = gotAccount[0].Id,
                Email = gotAccount[0].Email,
                FirstName = gotAccount[0].FirstName,
                LastName = gotAccount[0].LastName,
                Token = token
            };
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return StatusCode(500);
        }
    } 
}
