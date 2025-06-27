using Server.Models;

namespace Server.Services;

public interface IAccountService
{
    Task<Account?> GetAccount(string id);
    Task<bool> AddFunds(string id, double funds);
    Task<bool> ChangePassword(string id, string oldPassword, string newPassword);
    Task<Account?> UpdateAccount(Account account);
    Task<Order[]?> GetPurchases(string id);
}