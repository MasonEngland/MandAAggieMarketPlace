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
        Account? userAccont = await _db.accounts.FirstOrDefaultAsync(a => a.Id.ToString() == userId);
        if (userAccont == null)
        {
            return false;
        }

        Item? dbItem = await _db.CurrentStock.FirstOrDefaultAsync(i => i.Id == item.Id); // this is to keep entity trakcing in tact 
        if (dbItem == null)
        {
            return false;
        }

        CartItem cartItem = new CartItem
        {
            OrderItemId = dbItem,
            OwnerId = userId,
            Address = address,
            Amount = item.Stock
        };
        _db.CartItems.Add(cartItem);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Item[]> GetCartItems(string userId)
    {
        try
        {
            return await _db.CartItems
            .Where(ci => ci.OwnerId == userId)
            .Select(ci => ci.OrderItemId)
            .ToArrayAsync();
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return Array.Empty<Item>();
        }
        
    }

    public async Task<bool> PurchaseCartItems(string userId)
    {
        CartItem[] dbItems = await _db.CartItems.Where(p => Convert.ToString(p.OwnerId) == userId).ToArrayAsync();

        if (dbItems.Length < 1)
        {
            return false;
        }

        Item[] orderedItems = new Item[dbItems.Length];

        for (int i = 0; i < dbItems.Length; i++)
        {
            var newOrder = new Order()
            {
                OwnerId = userId,
                OrderItem = dbItems[i].OrderItemId,
                Address = dbItems[i].Address,
                Amount = dbItems[i].Amount
            };

            _db.OrderQueue.Add(newOrder);
            orderedItems[i] = dbItems[i].OrderItemId;
        }
        
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (Exception) { return false; }

        return true;
    }
}