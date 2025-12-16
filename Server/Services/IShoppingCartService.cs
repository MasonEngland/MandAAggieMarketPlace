using Server.Models;

namespace Server.Services;


public interface IShoppingCartService
{
    // returns true if adding was successful, false if not
    public Task<bool> AddToCart(Item item, string userId, string address);

    // returns all the items belonging to a given user
    public Task<Item[]> GetCartItems(string userId);

    // returns true if purchase was successful for all items
    // returns false if any items failed
    public Task<bool> PurchaseCartItems(string userId);

    public Task<bool> RemoveFromCart(string itemId, string userId);
}