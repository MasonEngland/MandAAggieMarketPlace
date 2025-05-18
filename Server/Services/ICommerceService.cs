using Server.Models;

namespace Server.Services;

public interface ICommerceService
{
    Task<(bool Success, Item? dbItem)> Purchase(Account account, Item item, string address);
    Task<bool> AddItem(Item item);
    Task<Item?> GetItem(string id);
}