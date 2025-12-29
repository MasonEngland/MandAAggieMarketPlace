using Server.Models;

public interface ITransactionService
{
    Task<object?> CreateCheckoutSession(double amount, Item[] items);

    bool HandleCheckoutStatus(string sessionId);

}