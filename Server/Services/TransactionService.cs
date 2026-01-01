using Microsoft.EntityFrameworkCore;
using Server.Context;
using Server.Models;
using Stripe.Checkout;

namespace Server.Services;

public class TransactionService : ITransactionService
{
    private readonly DatabaseContext _db;
    private readonly ICommerceService _commerceService;

    public TransactionService(DatabaseContext db, ICommerceService commerceService)
    {
        _db = db;
        _commerceService = commerceService;
    }

    public async Task<string?> CreateCheckoutSession(Item[] items)
    {
        string applicationUrl = Environment.GetEnvironmentVariable("APPLICATION_URL")!;
        List<SessionLineItemOptions> lineItems = new List<SessionLineItemOptions>();


        for (int i = 0; i< items.Length; i++)
        {
            Item item = items[i];
            Item dbItem = await _db.CurrentStock.FirstAsync(h => h.Id == item.Id);

            if (dbItem.Stock < item.Stock || item.Stock <= 0)
            {
                return null;
            }

            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Name,
                        Description = item.Description,
                    },
                },
                Quantity = item.Stock,
            });
        }

        var sessionoptions = new SessionCreateOptions
        {
            LineItems = lineItems,
            Mode = "payment",
            UiMode = "embedded",
            ReturnUrl = $"localhost:2501/Checkout_Status?session_id={{CHECKOUT_SESSION_ID}}" // enter url to handle checkout status

        };
        var sessionService = new SessionService();
        Session session = sessionService.Create(sessionoptions);
        return session.ClientSecret;

    }

    public async Task<bool> HandleCheckoutStatus(string sessionId, string accountId, Item[] items, string address)
    {
        var sessionService = new SessionService();
        Session session = sessionService.Get(sessionId);
        Account account = _db.accounts.First(a => a.Id.ToString() == accountId);


        if (session.PaymentStatus == "paid")
        {
            
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in items)
                {
                    (bool success, Item? item) result = await _commerceService.Purchase(account, item, address);

                    if (!result.success) throw new InvalidOperationException("Purchase failed");
                }
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
        return false;
    }
}