using Server.Context;
using Server.Models;
using Microsoft.EntityFrameworkCore;
using static Server.Util.SearchUtilities;
using System.Data.Common;
using System.Text;
using MySqlConnector;

namespace Server.Services;

public class CommerceService : ICommerceService
{
    private readonly DatabaseContext _db;

    public CommerceService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<Item?> GetItem(string id)
    {
        try
        {
            Item? item = await _db.CurrentStock.FirstOrDefaultAsync(h => Convert.ToString(h.Id) == id);
            return item;
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return null;
        }
    }

    public async Task<bool> AddItem(Item item)
    {
        // TODO: add a good admin check and remove the comment below
        // Account account = GetAccount()!;
        // if (account == null || !account.Admin) 
        // {
        //     return Redirect("/login");
        // }

        try
        {
            Item? dbResults = await _db.CurrentStock
                .FirstOrDefaultAsync(i => i.Id == item.Id || i.Name == item.Name);

            if (dbResults != null)
            {
                await _db.CurrentStock
                .Where(h => h.Id == item.Id || h.Name == item.Name)
                .ExecuteUpdateAsync(setter => setter.SetProperty(b => b.Stock, b => b.Stock + item.Stock));

                return true;
            }

            _db.CurrentStock.Add(item);
            await _db.SaveChangesAsync();

            return true;
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return false;
        }
    }

    public async Task<(bool Success, Item? dbItem)> Purchase(Account account, Item item, string address)
    {

        Item? dbItem = await _db.CurrentStock.FirstOrDefaultAsync(h => h.Id == item.Id);
        if (dbItem == null)
        {
            return (false, null);
        }

        if (dbItem.Stock < item.Stock || item.Stock <= 0)
        {
            return (false, null);
        }

        // lower the item stock by the amount requested
        dbItem.Stock -= item.Stock;

        try
        {
            _db.OrderQueue.Add(new Order()
            {
                OrderItem = dbItem,
                OwnerId = Convert.ToString(account.Id)!,
                Address = address,
                Amount = item.Stock
            }); 
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return (false, null);
        }
       
        await _db.SaveChangesAsync();

        return (true, dbItem);
    }

    public async Task<Item[]> GetItemsBySearch(string searchTerm, int page = 1, int pageSize = 10)
    {
        try
        {

            int maxDistance = Math.Min(5, searchTerm.Length / 3);
            List<Item> returnedItems = new List<Item>();




            // builds an SQL to query all items that share at least one character with the search term
            var sb = new StringBuilder();
            sb.Append("SELECT * FROM CurrentStock WHERE ");
            for (int i = 0; i < searchTerm.Length; i++)
            {
                if (i > 0)
                    sb.Append(" OR ");

                sb.Append($"Name LIKE CONCAT('%', @p{i}, '%')");
                sb.Append($" OR Description LIKE CONCAT('%', @p{i}, '%')");
                sb.Append($" OR Category LIKE CONCAT('%', @p{i}, '%')");
            }
            MySqlParameter[] parameters = searchTerm.Select((c, i) => new MySqlParameter($"@p{i}", c.ToString())).ToArray();
            Item[] searchCandidates = await _db.CurrentStock.FromSqlRaw(sb.ToString(), parameters).ToArrayAsync();



            if (searchCandidates.Length == 0)
            {
                return Array.Empty<Item>();
            }

            for (int i = 0; i < searchCandidates.Length; i++)
            {
                int nameDistance = PartialLevenshtein(searchTerm, searchCandidates[i].Name);
                int descriptionDistance = PartialLevenshtein(searchTerm, searchCandidates[i].Description);
                int categoryDistance = PartialLevenshtein(searchTerm, searchCandidates[i].Category);
                if (nameDistance < maxDistance || descriptionDistance < maxDistance || categoryDistance < maxDistance)
                {
                    returnedItems.Add(searchCandidates[i]);
                    continue;
                }
            }

            return returnedItems.ToArray();

        }
        catch (DbException err)
        {
            Console.WriteLine(err.Message);
            return Array.Empty<Item>();
        }
    }
}