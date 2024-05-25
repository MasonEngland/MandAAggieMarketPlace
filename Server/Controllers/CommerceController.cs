using System.Collections;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("Commerce")]
public class CommerceController : Controller
{
    private readonly DatabaseContext _db;
    public CommerceController(DatabaseContext db) 
    {
        _db = db;
    }

    [HttpPost("Purchase")]
    public object purchase([FromBody] Item item)
    {
        string? data = Convert.ToString(HttpContext.Items["tokenData"]);
        if (data == null) 
        {
            return Unauthorized();
        }

        Account? account = JsonSerializer.Deserialize<Account>(data!);

        if (account == null) 
        {
            return Unauthorized("token could not be deserialized");
        }

        try 
        {
            Item[] dbItem = _db.CurrentStock.Where(h => h.Id == item.Id).ToArray(); 
            if (dbItem.Length < 1) 
            {
                return NotFound("Item search failed");
            }

            _db.CurrentStock.Remove(dbItem[0]);
            _db.SaveChanges();

            _db.accounts
                .Where(item => item.Id == account.Id)
                .ExecuteUpdate(setter => setter.SetProperty(b => b.Balance, account.Balance - item.Price));


            
            //TODO : make a queue database
            //TODO : add purchased Item to the queue database
            //TODO : return a reciet noting the tansaction details

            return dbItem;
        } catch (Exception err) 
        {
            System.Console.WriteLine(err.Message);
            return StatusCode(500);
        }
    }
    
}
