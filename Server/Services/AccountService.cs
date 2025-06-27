using Server.Context;
using Server.Models;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;

namespace Server.Services;

public class AccountService : IAccountService
{
    private DatabaseContext _db;
    public AccountService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<bool> AddFunds(string id, double funds)
    {

        Account? account = await _db.accounts
            .Where(h => Convert.ToString(h.Id) == id).FirstOrDefaultAsync();

        if (account == null)
        {
            return false;
        }

        if (funds < 0 && Math.Abs(funds) > account.Balance)
        {
            return false;
        }

        account.Balance += (float) funds;
        _db.accounts.Update(account);
        await _db.SaveChangesAsync();


        return true;

    }

    public async Task<bool> ChangePassword(string id, string oldPassword, string newPassword)
    {
        try
        {
            // verify old password
            Account? account = await _db.accounts.Where(h => Convert.ToString(h.Id) == id).FirstOrDefaultAsync();
            if (account == null || !Verify(oldPassword, account.Password))
            {
                return false;
            }

            // update password
            await _db.accounts
            .Where(h => Convert.ToString(h.Id) == id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(b => b.Password, HashPassword(newPassword))
            );
            return true;
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return false;
        }

    }

    public async Task<Account?> GetAccount(string id)
    {
        try
        {
            Account account = (await _db.accounts
            .Where(h => Convert.ToString(h.Id) == id)
            .ToArrayAsync())[0];

            return account;
        } catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return null;
        }
    }

    public async Task<Order[]?> GetPurchases(string id)
    {
        Order[] Items = await _db.OrderQueue.Where(b => b.OwnerId == Convert.ToString(id)).ToArrayAsync();

        if (Items.Length == 0)
        {
            return null;
        }
        return Items;
    }

    public async Task<Account?> UpdateAccount(Account account)
    {
        Account? gotAccount = await _db.accounts
           .Where(h => h.Id == account.Id)
           .FirstOrDefaultAsync();

        if (gotAccount == null)
        {
            return null;
        }
        gotAccount.FirstName = account.FirstName;
        gotAccount.LastName = account.LastName;
        gotAccount.Email = account.Email;
        _db.accounts.Update(gotAccount);
        await _db.SaveChangesAsync();

    
        return gotAccount;
    }
}
