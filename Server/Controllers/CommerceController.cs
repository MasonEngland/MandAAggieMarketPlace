using Microsoft.AspNetCore.Mvc;
using Server.Context;
using Server.Models;
using Server.Services;
using static Server.Util.AccountUtilities;

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

    [HttpGet("Search/{searchTerm}")]
    public async Task<IActionResult> SearchItems(string searchTerm)
    {
        /*
         * this will return all the items that match the search term
         * if no items match then it will return an empty array
         * 
         * @param searchTerm - String
         * the term to search for in the item name or description
         * 
         * @param page - int
         * the page number to return, defaults to 1
         * 
         * @param pageSize - int
         * the number of items to return per page, defaults to 10
         */
        try 
        {
            Item[] items = await _commerceService.GetItemsBySearch(searchTerm);
            return Ok(new 
            {
                stock = items,
                success = true
            });
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return StatusCode(500, new
            {
                success = false,
                message = "Error searching for items"
            });
        }
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

    //! this code is there to stock the database using fakestore API
    //! do not delete and only uncomment for use
    // [HttpGet("StockDb/GetStock")]
    // public async Task<IActionResult> SyncItemsFromApi()
    // {
    //     try
    //     {
    //         // Fetch items from the FakeStore API
    //         var items = await FetchItemsFromApiAsync();

    //         // Add each item to the database
    //         _db.CurrentStock.AddRange(items);

    //         // Save changes to the database
    //         await _db.SaveChangesAsync();

    //         return Ok(new { message = "Items successfully added to the database." });
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, new { message = ex.Message });
    //     }
    // }

    // private async Task<List<Item>> FetchItemsFromApiAsync()
    // {
    //     using (var client = new HttpClient())
    //     {
    //         // Call the FakeStore API
    //         var response = await client.GetStringAsync("https://fakestoreapi.com/products");

    //         // Deserialize the response into a list of FakeStoreProduct objects
    //         var fakeStoreProducts = JsonConvert.DeserializeObject<List<dynamic>>(response)!;

    //         // Map FakeStoreProduct to Item
    //         var items = fakeStoreProducts.Select(p => new Item
    //         {
    //             Name = p.title,
    //             Stock = 100, // You might want to change this logic or add your own stock management
    //             Description = p.description,
    //             Price = p.price,
    //             ImageLink = p.image,
    //             Category = p.category
    //         }).ToList();

    //         return items;
    //     }
    // }
}


