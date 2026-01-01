using Server.Models;

public interface ITransactionService
{
    Task<string?> CreateCheckoutSession(Item[] items);

    Task<bool> HandleCheckoutStatus(string sessionId, string accountId, Item[] items, string address);

}