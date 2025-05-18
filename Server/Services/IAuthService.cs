
using Server.Models;

namespace Server.Services;

public interface IAuthService
{
    Task<Account?> CreateUser(Account account);
    Task<Account?> AuthenticateUser(Account account);

    Task<bool> LogoutUser(string token);
}