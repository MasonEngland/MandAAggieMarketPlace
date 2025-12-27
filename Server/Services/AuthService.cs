using Server.Models;
using Server.Context;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;

namespace Server.Services;

public class AuthService : IAuthService
{
    private readonly DatabaseContext _db;
    
    public AuthService(DatabaseContext db)
    {
        _db = db;
    }

    public Task<Account?> AuthenticateUser(Account account)
    {
        throw new NotImplementedException();
    }

    public async Task<Account?> CreateUser(Account account)
    {
        try
        {
            Account? dbResults = await _db.accounts
                .FirstOrDefaultAsync(a => a.Email == account.Email);

            if (dbResults != null)
            {
                return dbResults;
            }
            account.Password = HashPassword(account.Password);

            _db.accounts.Add(account);
            await _db.SaveChangesAsync();

            return account;
        }
        catch (Exception err)
        {
            Console.WriteLine(err.Message);
            return null;
        }
    }

    public Task<bool> LogoutUser(string token)
    {
        throw new NotImplementedException();
    }
}