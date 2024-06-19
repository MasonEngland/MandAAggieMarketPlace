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

    [HttpPost("Purchase/{address}")]
    public object purchase([FromBody] Item item, String address)    
    {
        Account? account = GetAccount();
        if (account == null)    
        {
            return Unauthorized("token could not be deserialized");
        }

        if (account.Balance - item.Price < 0)
        {
            return new 
            {
                success = false,
                message = "Not enough funds"
            };
        }

        try 
        {
            Item[] dbItem = _db.CurrentStock.Where(h => h.Id == item.Id).ToArray(); 
            if (dbItem.Length < 1) 
            {
                return NotFound("Item search failed");
            } 

            if (item.Stock > dbItem[0].Stock || item.Stock == 0)
            {
                return new 
                {
                    success = false,
                    message = "Not enough items in stock"
                };
            }

            _db.CurrentStock
                .Where(h => h.Id == item.Id)
                .ExecuteUpdate(setter => setter.SetProperty(b => b.Stock, b => b.Stock - item.Stock));
         
            // remove the money from the clients account to pay for the product 
            _db.accounts
                .Where(item => item.Id == account.Id)
                .ExecuteUpdate(setter => setter.SetProperty(b => b.Balance, account.Balance - item.Price));

            
            _db.OrderQueue.Add(new Order() {
                OrderItem = item,
                Adress = address
            });
            _db.SaveChanges();

            
            //return a reciet noting the tansaction details
            return new 
            {
                orderedItem = dbItem[0],
                addressTo = address,
                cost = -dbItem[0].Price,
                success = true
            };
        } catch (Exception err) 
        {
            System.Console.WriteLine(err.Message);
            return StatusCode(500);
        }
    }

    [HttpPost("Restock")]
    public object AddToCurrentStock([FromBody] Item item)
    {
        try 
        {
            Item[] dbResults = _db.CurrentStock
                .Where(i => i.Id == item.Id || i.Name == item.Name)
                .ToArray();

            if (dbResults.Length > 1)
            {
                _db.CurrentStock
                .Where(h => h.Id == item.Id || h.Name == item.Name)
                .ExecuteUpdate(setter => setter.SetProperty(b => b.Stock, b => b.Stock + 1));

                return new 
                {
                    success = true
                };
            }

            _db.CurrentStock.Add(item);
            _db.SaveChanges();

            return new 
            {
                success = true 
            };
            
        } catch (Exception err) 
        {
            Console.WriteLine(err.Message);
            HttpContext.Response.StatusCode = 500;
            return new 
            {
                success = false
            };
        }
    } 
}
