using Microsoft.EntityFrameworkCore;
using Server.Context;
using Server.Models;

namespace Server.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly DatabaseContext _db;
    public ShoppingCartService(DatabaseContext db)
    {
        _db = db;
    }
    public async Task<bool> AddToCart(Item item, string userId, string address)
    {
        /*
        * only returns true if the item request is succefully added to the users shopping cart 
        */

        Account? userAccount = await _db.accounts.FirstOrDefaultAsync(a => a.Id.ToString() == userId);
        if (userAccount == null)
        {
            return false;
        }

        Item? dbItem = await _db.CurrentStock.FirstOrDefaultAsync(i => i.Id == item.Id); // this is to keep entity trakcing in tact 
        if (dbItem == null)
        {
            return false;
        }

        if (dbItem.Stock <= 0) return false;



        CartItem cartItem = new CartItem
        {
            OrderItem = dbItem,
            OwnerId = userId,
            Address = address,
            Amount = item.Stock
        };
        _db.CartItems.Add(cartItem);

        try { await _db.SaveChangesAsync(); }
        catch (Exception err) { Console.WriteLine(err.Message);  return false; }

        return true;
    }

    public async Task<Item[]> GetCartItems(string userId)
    {
        try
        {
            var cartItems = await _db.CartItems
                .Where(ci => ci.OwnerId == userId)
                .ToArrayAsync();


            var stockItems = await _db.CartItems
                .Where(ci => ci.OwnerId == userId)
                .Select(ci => ci.OrderItem)
                .ToArrayAsync();

            for (int i = 0; i < stockItems.Length; i++)
            {
                stockItems[i].Stock = cartItems[i].Amount;
            }
            
            return stockItems;
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return Array.Empty<Item>();
        }
        
    }

    public async Task<bool> PurchaseCartItems(string userId)
    {
        /**
        * Method to purchase ALL items in a users shopping cart
        * will ONLY return true if all the items are purchased succesfully
        * users will have the opportunity to purchase items from their cart individually as well 
        */


        CartItem[] dbItems = await _db.CartItems.Where(p => p.OwnerId == userId).ToArrayAsync();
        Account? user = await _db.accounts.Where(p => p.Id.ToString() == userId).FirstOrDefaultAsync();

        if (user == null) return false;


        if (dbItems.Length < 1)
        {
            return false;
        }

        Item[] orderedItems = new Item[dbItems.Length];

        for (int i = 0; i < dbItems.Length; i++)
        {

            if (user.Balance < dbItems[i].OrderItem.Price) return false;
            user.Balance -= dbItems[i].OrderItem.Price;

            if (dbItems[i].OrderItem.Stock < dbItems[i].Amount) return false;
            dbItems[i].OrderItem.Stock -= dbItems[i].Amount;


            var newOrder = new Order()
            {
                OwnerId = userId,
                OrderItem = dbItems[i].OrderItem,
                Address = dbItems[i].Address,
                Amount = dbItems[i].Amount
            };

            _db.OrderQueue.Add(newOrder);
            orderedItems[i] = dbItems[i].OrderItem;
            _db.CartItems.Remove(dbItems[i]);
        }


        await _db.SaveChangesAsync();


        return true;
    }
    
    public async Task<bool> RemoveFromCart(string itemId, string userId)
    {
        try
        {
            CartItem? cartItem = await _db.CartItems
                .FirstOrDefaultAsync(ci => ci.OwnerId == userId && ci.OrderItem.Id.ToString() == itemId);

            if (cartItem == null)
            {
                return false;
            }

            _db.CartItems.Remove(cartItem);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return false;
        }
    }
}