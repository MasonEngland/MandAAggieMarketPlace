using Server.Models;

namespace Server.Services;

public interface ICommerceService
{
    Task<(bool Success, Item? dbItem)> Purchase(Account account, Item item, string address);
    Task<bool> AddItem(Item item);
    Task<Item?> GetItem(string id);

    Task<Item[]> GetItemsBySearch(string searchTerm, int page = 1, int pageSize = 10);
}