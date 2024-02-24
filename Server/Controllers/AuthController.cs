using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Server.Models;
using static BCrypt.Net.BCrypt;

namespace Server.Controllers;

[ApiController]
[Route("Auth")]
public class AuthController : Controller
{
    private readonly DatabaseContext _db;
    public AuthController(DatabaseContext db) 
    {
        _db = db;
    }

    [HttpPost("Create")]
    public object create([FromBody] Account account)
    {
        if (account.Email == null) 
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
                Balace = 0.0f
                
            });
            _db.SaveChanges();

            return StatusCode(201);
            
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return StatusCode(500);
        }

    }

    [HttpGet]
    public string test()
    {
        return "it worked";
    }

}
