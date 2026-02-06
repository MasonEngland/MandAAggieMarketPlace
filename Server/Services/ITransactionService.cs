using Server.Models;

public interface ITransactionService
{
    Task<string?> CreateCheckoutSession(CartItem[] items, string address);

    Task<bool> HandleCheckoutStatus(string sessionId, string accountId, string address);

    Task<string?> GetSecret(string sessionId);

}