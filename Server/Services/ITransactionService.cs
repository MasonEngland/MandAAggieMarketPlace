using Server.Models;

public interface ITransactionService
{
    Task<string?> CreateCheckoutSession(Item item);

    Task<bool> HandleCheckoutStatus(string sessionId, string accountId, string address);

    Task<string?> GetSecret(string sessionId);

}