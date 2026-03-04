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

        // check if item is already in cart, if so update the amount instead of adding a new cart item
        CartItem? existingCartItem = await _db.CartItems
            .FirstOrDefaultAsync(ci => ci.OwnerId == userId && ci.OrderItemId == item.Id);
        
        if (existingCartItem != null)
        {
            existingCartItem.Amount += item.Stock;
            try { await _db.SaveChangesAsync(); }
            catch (Exception err) { Console.WriteLine(err.Message); return false; }
            return true;
        }

        Account? userAccount = await _db.accounts.FirstOrDefaultAsync(a => a.Id.ToString() == userId);
        if (userAccount == null)
        {
            return false;
        }

        Item? dbItem = await _db.CurrentStock.FirstOrDefaultAsync(i => i.Id == item.Id); // this is to keep entity tracking in tact 
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

    public async Task<CartItem[]> GetCartItems(string userId)
    {
        try
        {
            CartItem[] cartItems = await _db.CartItems
                .Where(ci => ci.OwnerId == userId)
                .ToArrayAsync();

            
            return cartItems;
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return Array.Empty<CartItem>();
        }
        
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