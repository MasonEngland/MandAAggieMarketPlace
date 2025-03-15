using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("Api/Commerce")]
public class CommerceController : Controller
{
    private readonly DatabaseContext _db;

    public CommerceController(DatabaseContext db) 
    {
        _db = db;
    }

    // Helper function for deserializing the account info from token data  
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

    [HttpGet("GetStock/{queries}")]
    public object GetStock(int queries) 
    {
        /**
         * this will return all the store items up to the amount of queries requested
         * if there are less than 10 items in the stock then it will return the full stock 
         *  of the store
         * 
         * */
        try 
        {
            List<Item> stock = _db.CurrentStock.Take(10 * queries).ToList();
            return new 
            {
                stock,
                success = true
            };
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return StatusCode(500);
        }
    }
    

    [HttpPost("Purchase/{address}")]
    public object Purchase([FromBody] Item item, String address)    
    {
        /*
         *
         * used when a logged in user purchases an items
         * authorzation token is required through the auth header 
         *
         * @param address - String 
         * the address will be added to the body of the orders that will get loaded from a separate front end
         *
         * @param item - Item instance
         * will get sent through the request body and should include all the fields except for Date and Id 
         * If an Id is provided then the method will search for an item with that Id, if not it will use the name as 
         * the search parameters
         *
         *
         */

        Account? account = GetAccount();
        if (account == null)    
        {
            return Redirect("/login");
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
                HttpContext.Response.StatusCode = 400;
                return new 
                {
                    success = false,
                    message = "Not enough items in stock"
                };
            }

            // lower the item stock by the amount requested
            _db.CurrentStock
                .Where(h => h.Id == item.Id)
                .ExecuteUpdate(setter => setter.SetProperty(b => b.Stock, b => b.Stock - item.Stock));

            // remove the money from the clients account to pay for the product 
            _db.accounts
                .Where(item => item.Id == account.Id)
                .ExecuteUpdate(setter => setter.SetProperty(b => b.Balance, account.Balance - item.Price));

            
            _db.OrderQueue.Add(new Order() {
                OrderItem = item,
                OwnerId = Convert.ToString(account.Id)!, 
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
            /**
             * check if the requested item exists 
             * if the item exists then add 1 to the stock 
             * if not, then add the item to the database
             *
             * adds as many items as the value of Item.stock in the request body 
             */

            // TODO: add a good admin check and remove the comment below
            // Account account = GetAccount()!;
            // if (account == null || !account.Admin) 
            // {
            //     return Redirect("/login");
            // }
            
            
            Item[] dbResults = _db.CurrentStock
                .Where(i => i.Id == item.Id || i.Name == item.Name)
                .ToArray();

            if (dbResults.Length >= 1)
            {
                _db.CurrentStock
                .Where(h => h.Id == item.Id || h.Name == item.Name)
                .ExecuteUpdate(setter => setter.SetProperty(b => b.Stock, b => b.Stock + item.Stock));

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
