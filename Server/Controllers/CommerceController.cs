using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Server.Models;
using Microsoft.EntityFrameworkCore;
using Server.Services;
using static Server.Util.AccountUtilities;
using Microsoft.Extensions.ObjectPool;

namespace Server.Controllers;

[ApiController]
[Route("Api/Commerce")]
public class CommerceController : Controller
{
    private readonly DatabaseContext _db;
    private readonly ICommerceService _commerceService;


    public CommerceController(DatabaseContext db, ICommerceService commerceService)
    {
        _db = db;
        _commerceService = commerceService;
    }
    

    // Helper function for deserializing the account info from token data  
    

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
    public async Task<IActionResult> Purchase([FromBody] Item item, string address)    
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

        Account? account = GetAccount(HttpContext);
        if (account == null)    
        {
            return Redirect("/login");
        }

        (bool Success, Item? dbItem) result = await _commerceService.Purchase(account, item, address);

        if (!result.Success || result.dbItem == null) 
        {
            return BadRequest(new 
            {
                success = false,
                message = "failed to purchase item"
            });
        }

        
        //return a reciet noting the tansaction details
        return Ok(new
        {
            orderedItem = result.dbItem,
            addressTo = address,
            cost = result.dbItem.Price,
            success = true,
            message = "purchase successful"
        });
        
    }

    [HttpPost("Restock")]
    public async Task<IActionResult> AddToCurrentStock([FromBody] Item item)
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
            bool result = await _commerceService.AddItem(item);
            if (!result) 
            {
                return BadRequest(new 
                {
                    success = false,
                    message = "failed to add item to stock"
                });
            }

            return Created($"/Api/Commerce/GetItem/{item.Id}", new
            {
                success = true
            });
            
        } catch (Exception err) 
        {
            Console.WriteLine(err.Message);
            return StatusCode(500, new {
                success = false,
                message = "Error adding item to stock"
            });
        }
    } 

    [HttpGet("GetItem/{id}")]
    public async Task<IActionResult> GetItem(string id) 
    {
        Item? item = await _commerceService.GetItem(id);
        if (item == null) 
        {
            return NotFound(new 
            {
                success = false,
                message = "item not found"
            });
        }

        return Ok(new
        {
            item,
            success = true
        });
        
    }
}
