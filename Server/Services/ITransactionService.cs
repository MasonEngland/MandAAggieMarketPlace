using Server.Models;

public interface ITransactionService
{
    Task<object?> CreateCheckoutSession(double amount, Item[] items);

    Task<bool> HandleCheckoutStatus(string sessionId, string accountId, Item[] items, string address);

}